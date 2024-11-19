// User Defined
#include <iomanip>
#include <server.h>
#include <packet_type.h>

bool Server::__M_InitializeENet() const
{
    int result = enet_initialize();
    atexit(enet_deinitialize);
    return result == 0;
}

ENetHost *Server::__M_CreateServer() const
{
    ENetAddress address;
    enet_address_set_ip(&address, "127.0.0.1");
    address.port = PORT;

    ENetHost *server;
    server = enet_host_create(&address,
                              MAX_CONNECTIONS, // allow up to N clients and/or outgoing connections
                              NUM_CHANNELS,    // allow up to N channels to be used
                              0,               // assume any amount of incoming bandwidth
                              0,               // assume any amount of outgoing bandwidth
                              ENET_HOST_BUFFER_SIZE_MIN);
    return server;
}

std::string Server::__M_CurrentTime() const
{
    auto now = std::chrono::system_clock::now();
    std::time_t time = std::chrono::system_clock::to_time_t(now);
    std::stringstream ss;
    ss << std::put_time(std::localtime(&time), "%H:%M:%S");
    return ss.str();
}

void Server::__M_Send(ENetPeer *peer, const Message &message)
{
    ENetPacket *packet = enet_packet_create(
        message.str().c_str(),
        message.size(),
        ENET_PACKET_FLAG_RELIABLE);

    enet_peer_send(peer, 0, packet);
    enet_host_flush(peer->host);
}

void Server::__M_ParsePacket(const ENetEvent &event) const
{
    const uint8_t *data = event.packet->data;
    const uint32_t &dataLength = event.packet->dataLength;

    // length is valid
    if (dataLength <= 0)
        return;

    // event type is valid
    uint8_t eventType = data[0];
    if (eventType >= EventType::INVALID_EVENT)
        return;

    // TEMP FUNCTION
    __M_Log("Data: ", data[1]);
}

Server::Server()
    : m_buffer(std::make_unique<char[]>(BUFFER_SIZE))
{
    if (!__M_InitializeENet())
    {
        __M_LogError("An error occurred while initializing ENet.");
        return;
    }

    m_server = __M_CreateServer();
    if (m_server == NULL)
    {
        __M_LogError("An error occurred while trying to create an ENet server host.");
        return;
    }

    m_isRunning = true;

    __M_Log("Server started on port ", PORT);
}

Server::~Server()
{
    for (auto &client : m_clients)
    {
        ENetPeer *peer = client.second;
        if (peer && peer->state == ENET_PEER_STATE_CONNECTED)
            enet_peer_disconnect_now(peer, 0);
    }

    m_clients.clear();
    enet_host_destroy(m_server);
}

void Server::tick()
{
    ENetEvent event;
    while (true)
    {
        int32_t eventSize = enet_host_service(m_server, &event, 0);

        if (eventSize == 0)
            break;

        if (eventSize < 0)
        {
            __M_LogError("Encountered error while polling");
            break;
        }

        switch (event.type)
        {
        case ENET_EVENT_TYPE_RECEIVE:
            __M_Log("Client #", event.peer->incomingPeerID, " received message");
            __M_ParsePacket(event);
            break;
        case ENET_EVENT_TYPE_CONNECT:
            __M_Log("Client #", event.peer->incomingPeerID, " connected to the server.");
            m_clients[event.peer->incomingPeerID] = event.peer;
            __M_Send(event.peer, {EventType::CONNECTION_SUCCEED, "Welcome to the server!"});
            break;
        case ENET_EVENT_TYPE_DISCONNECT:
            __M_Log("Client #", event.peer->incomingPeerID, " disconnected from the server.");
            m_clients.erase(event.peer->incomingPeerID);
            break;
        default:
            break;
        }
    }
}

void Server::send(uint32_t id, const Message &message)
{
    if (m_clients.find(id) != m_clients.end())
        __M_Send(m_clients[id], message);
}

bool Server::isRunning() const
{
    return m_isRunning;
}
