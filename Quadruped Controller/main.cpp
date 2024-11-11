#define WIN32_LEAN_AND_MEAN

#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>
#include <thread>

const int PORT = 5000;
const int BUFFER_SIZE = 1024;

bool InitializeWinSock()
{
    WSADATA wsaData;
    int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
    return result == 0;
}

SOCKET CreateNonBlockingSocket()
{
    SOCKET listeningSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (listeningSocket == INVALID_SOCKET)
    {
        std::cerr << "Error creating socket: " << WSAGetLastError() << std::endl;
        return INVALID_SOCKET;
    }

    u_long mode = 1;
    ioctlsocket(listeningSocket, FIONBIO, &mode);

    return listeningSocket;
}

bool BindAndListen(SOCKET &listeningSocket)
{
    sockaddr_in serverAddr = {};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");
    serverAddr.sin_port = htons(PORT);

    if (bind(listeningSocket, (sockaddr *)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
    {
        std::cerr << "Bind failed: " << WSAGetLastError() << std::endl;
        closesocket(listeningSocket);
        return false;
    }

    if (listen(listeningSocket, SOMAXCONN) == SOCKET_ERROR)
    {
        std::cerr << "Listen failed: " << WSAGetLastError() << std::endl;
        closesocket(listeningSocket);
        return false;
    }

    return true;
}

void RunServer()
{
    if (!InitializeWinSock())
    {
        std::cerr << "WinSock initialization failed." << std::endl;
        return;
    }

    SOCKET listeningSocket = CreateNonBlockingSocket();
    if (listeningSocket == INVALID_SOCKET)
    {
        WSACleanup();
        return;
    }

    if (!BindAndListen(listeningSocket))
    {
        WSACleanup();
        return;
    }

    std::cout << "Server started on port " << PORT << std::endl;

    char buffer[BUFFER_SIZE];
    bool isRunning = true;

    while (isRunning)
    {
        SOCKET clientSocket = accept(listeningSocket, nullptr, nullptr);
        if (clientSocket != INVALID_SOCKET)
        {
            std::cout << "Client connected." << std::endl;

            u_long mode = 1;
            ioctlsocket(clientSocket, FIONBIO, &mode);

            while (true)
            {
                int bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE, 0);
                if (bytesReceived > 0)
                {
                    std::string receivedMessage(buffer, bytesReceived);
                    std::cout << "Received: " << receivedMessage << std::endl;

                    std::string response = "Response from server: " + receivedMessage;
                    send(clientSocket, response.c_str(), response.size(), 0);
                }
                else if (bytesReceived == SOCKET_ERROR && WSAGetLastError() == WSAEWOULDBLOCK)
                {
                    std::this_thread::sleep_for(std::chrono::milliseconds(50));
                }
                else
                {
                    std::cout << "Client disconnected." << std::endl;
                    closesocket(clientSocket);
                    break;
                }
            }
        }
        else
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(50));
        }
    }

    closesocket(listeningSocket);
    WSACleanup();
}

int main()
{
    RunServer();
    return 0;
}