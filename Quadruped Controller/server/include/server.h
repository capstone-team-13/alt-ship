#include <chrono>
#include <iostream>
#include <sstream>
#include <memory>
#include <vector>
#include <unordered_map>
#include <enet.h>
#include <message.h>

class Server
{
    const int PORT = 5000;
    const int BUFFER_SIZE = 1024;

    const uint32_t MAX_CONNECTIONS = 64;
    const uint8_t NUM_CHANNELS = 2;

    ENetHost *m_server;
    std::unordered_map<uint32_t, ENetPeer *> m_clients;
    bool m_isRunning;
    std::unique_ptr<char[]> m_buffer;

    bool __M_InitializeENet() const;
    ENetHost *__M_CreateServer() const;
    std::string __M_CurrentTime() const;
    void send(ENetPeer *peer, const Message &message);

    template <typename... Args>
    void __M_Log(const Args &...args) const;

    template <typename... Args>
    void __M_LogError(const Args &...args) const;

public:
    Server();
    ~Server();
    void tick();
    void Poll();
    bool isRunning() const;
};

template <typename... Args>
inline void Server::__M_Log(const Args &...args) const
{
    std::stringstream ss;
    ss << "[" << __M_CurrentTime() << "] ";
    ss << "[Server] ";
    (ss << ... << args);
    std::cout << ss.str() << std::endl;
}

template <typename... Args>
inline void Server::__M_LogError(const Args &...args) const
{
    std::stringstream ss;
    ss << "[" << __M_CurrentTime() << "] ";
    ss << "[Server] ";
    (ss << ... << args);
    std::cerr << ss.str() << std::endl;
}