#include <ws2tcpip.h>
#include <thread>
#include "server.h"

bool Server::__M_InitializeWinSock() const
{
    WSADATA wsaData;
    int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
    return result == 0;
}

SOCKET Server::__M_CreateSocket() const
{
    SOCKET socketId = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (socketId == INVALID_SOCKET)
    {
        __M_LogError("Error creating socket: ", WSAGetLastError());
        return INVALID_SOCKET;
    }

    u_long mode = 1;
    ioctlsocket(socketId, FIONBIO, &mode);

    return socketId;
}

bool Server::__M_Bind(SOCKET &socketId) const
{
    sockaddr_in serverAddr = {};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");
    serverAddr.sin_port = htons(PORT);

    if (bind(socketId, (sockaddr *)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
    {
        __M_LogError("Bind failed: ", WSAGetLastError());
        closesocket(socketId);
        return false;
    }

    return true;
}

bool Server::__M_Listen(SOCKET &socketId) const
{
    if (listen(socketId, SOMAXCONN) == SOCKET_ERROR)
    {
        __M_LogError("Listen failed: ", WSAGetLastError());
        closesocket(socketId);
        return false;
    }

    return true;
}

Server::Server()
    : m_clients(),
      m_buffer(std::make_unique<char[]>(BUFFER_SIZE)),
      m_socketId(INVALID_SOCKET)
{
    if (!__M_InitializeWinSock())
    {
        __M_LogError("WinSock initialization failed.");
        return;
    }

    SOCKET socketId = __M_CreateSocket();
    if (socketId == INVALID_SOCKET || !__M_Bind(socketId) || !__M_Listen(socketId))
    {
        WSACleanup();
        return;
    }

    m_socketId = socketId;
    m_isRunning = true;

    std::cout << "[Server] Server started on port " << PORT << std::endl;
}

Server::~Server()
{
    for (SOCKET clientSocket : m_clients)
    {
        closesocket(clientSocket);
    }

    closesocket(m_socketId);
    WSACleanup();
}

void Server::Tick()
{
    SOCKET clientSocket = accept(m_socketId, nullptr, nullptr);
    if (clientSocket != INVALID_SOCKET)
    {
        __M_Log("Client connected.");

        // Set the client socket to non-blocking mode
        u_long mode = 1;
        ioctlsocket(clientSocket, FIONBIO, &mode);

        m_clients.push_back(clientSocket);
        __M_Log("Client Count: ", m_clients.size());
    }

    for (auto it = m_clients.begin(); it != m_clients.end();)
    {
        SOCKET clientId = *it;

        int bytesReceived = recv(clientId, m_buffer.get(), BUFFER_SIZE, 0);
        if (bytesReceived > 0)
        {
            std::string receivedMessage(m_buffer.get(), bytesReceived);
            __M_Log("Received from client: ", receivedMessage);

            std::string response = "Response from server: " + receivedMessage;
            send(clientId, response.c_str(), response.size(), 0);
        }
        else if (bytesReceived == SOCKET_ERROR && WSAGetLastError() == WSAEWOULDBLOCK)
        {
            ++it;
        }
        else
        {
            __M_Log("Client disconnected.");
            closesocket(clientId);
            it = m_clients.erase(it);
        }
    }

    std::this_thread::sleep_for(std::chrono::milliseconds(100));
}

bool Server::IsRunning() const
{
    return m_isRunning;
}
