#pragma once

#include <chrono>
#include <message.h>
#include <eventpp/callbacklist.h>

class Server
{
#pragma region "API"
public:
    typedef eventpp::CallbackList<void(const ENetEvent &, uint8_t, const uint8_t *, uint32_t)> PacketReceivedEvent;

    Server();
    ~Server();
    void tick();
    void send(uint32_t id, const Message &message);
    bool isRunning() const;
    PacketReceivedEvent::Handle addPacketReceivedCallback(const PacketReceivedEvent::Callback &callback);
    bool removePacketReceivedCallback(const PacketReceivedEvent::Handle &handle);

#pragma endregion

private:
    const int PORT = 5000;
    const int BUFFER_SIZE = 1024;

    const uint32_t MAX_CONNECTIONS = 64;
    const uint8_t NUM_CHANNELS = 2;

    ENetHost *m_server;
    std::unordered_map<uint32_t, ENetPeer *> m_clients;
    std::atomic<bool> m_isRunning;
    std::unique_ptr<char[]> m_buffer;

    PacketReceivedEvent m_onReceivePacket;

    bool __M_InitializeENet() const;
    ENetHost *__M_CreateServer() const;
    std::string __M_CurrentTime() const;

    void __M_Send(ENetPeer *peer, const Message &message);
    void __M_ParsePacket(const ENetEvent &event) const;

    // TODO: Refactor as a logger class
public:
    template <typename... Args>
    void __M_Log(const Args &...args) const;

    template <typename... Args>
    void __M_LogError(const Args &...args) const;
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