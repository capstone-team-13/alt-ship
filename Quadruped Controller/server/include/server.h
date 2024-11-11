#define WIN32_LEAN_AND_MEAN
#include <winsock2.h>
#include <iostream>
#include <sstream>
#include <memory>
#include <vector>

class Server
{
    const int PORT = 5000;
    const int BUFFER_SIZE = 1024;

    SOCKET m_socketId;
    bool m_isRunning;

    std::vector<SOCKET> m_clients;
    std::unique_ptr<char[]> m_buffer;

    bool __M_InitializeWinSock() const;
    SOCKET __M_CreateSocket() const;

    bool __M_Bind(SOCKET &listeningSocket) const;
    bool __M_Listen(SOCKET &listeningSocket) const;

    template <typename... Args>
    void __M_Log(const Args &...args) const;

    template <typename... Args>
    void __M_LogError(const Args &...args) const;

public:
    Server();
    ~Server();
    void Tick();
    bool IsRunning() const;
};

template <typename... Args>
inline void Server::__M_Log(const Args &...args) const
{
    std::stringstream ss;
    ss << "[Server] ";
    (ss << ... << args);
    std::cout << ss.str() << std::endl;
}

template <typename... Args>
inline void Server::__M_LogError(const Args &...args) const
{
    std::stringstream ss;
    ss << "[Server] ";
    (ss << ... << args);
    std::cerr << ss.str() << std::endl;
}
