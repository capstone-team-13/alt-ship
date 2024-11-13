// User Defined
#include <iomanip>
#include <server.h>

bool Server::__M_InitializeENet() const
{
    int result = enet_initialize();
    atexit(enet_deinitialize);
    return result == 0;
}

ENetHost *Server::__M_CreateServer() const
{
    ENetAddress address;
    enet_address_set_host(&address, "127.0.0.1");
    address.port = PORT;

    ENetHost *server;
    server = enet_host_create(&address,
                              MAX_CONNECTIONS, // allow up to N clients and/or outgoing connections
                              NUM_CHANNELS,    // allow up to N channels to be used
                              0,               // assume any amount of incoming bandwidth
                              0);              // assume any amount of outgoing bandwidth
    return server;
}

std::string Server::__M_CurrentTime() const
{
    auto now = std::chrono::system_clock::now();
    std::time_t time = std::chrono::system_clock::to_time_t(now);
    std::stringstream ss;
    ss << std::put_time(std::localtime(&time), "%Y-%m-%d %H:%M:%S");
    return ss.str();
}

void Server::SendTo(ENetPeer *peer, const std::string &message)
{
    ENetPacket *packet = enet_packet_create(
        message.c_str(),
        message.size() + 1,
        ENET_PACKET_FLAG_RELIABLE);

    enet_peer_send(peer, 0, packet);
    enet_host_flush(peer->host);
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
    enet_host_destroy(m_server);
}

void Server::Tick()
{
    // ENetEvent event;
    // while (true)
    // {
    //     int32_t eventSize = enet_host_service(m_server, &event, 0);

    //     if (eventSize == 0)
    //         break;

    //     if (eventSize < 0)
    //     {
    //         __M_LogError("Encountered error while polling");
    //         break;
    //     }

    //     switch (event.type)
    //     {
    //     case ENET_EVENT_TYPE_RECEIVE:
    //         __M_Log("Client #", event.peer->incomingPeerID, " recevied message");
    //         break;
    //     case ENET_EVENT_TYPE_CONNECT:
    //         __M_Log("Client #", event.peer->incomingPeerID, " connected to the server.");
    //         m_clients[event.peer->incomingPeerID] = event.peer;
    //         SendTo(event.peer, "Welcome to the server!");
    //         break;
    //     case ENET_EVENT_TYPE_DISCONNECT:
    //         __M_Log("Client #", event.peer->incomingPeerID, " disconnected from the server.");
    //         m_clients.erase(event.peer->incomingPeerID);
    //         SendTo(event.peer, "Goodbye!");
    //         break;
    //     default:
    //         break;
    //     }
    // }

    ENetEvent event;
    while (enet_host_service(m_server, &event, 1000) > 0)
    {
        switch (event.type)
        {
        case ENET_EVENT_TYPE_RECEIVE:
            std::cout << "Received a packet of length " << event.packet->dataLength << " from client.\n";
            enet_packet_destroy(event.packet);
            break;
        case ENET_EVENT_TYPE_CONNECT:
            std::cout << "Client connected.\n";
            break;
        case ENET_EVENT_TYPE_DISCONNECT:
            std::cout << "Client disconnected.\n";
            break;
        }
    }
}

bool Server::IsRunning() const
{
    return m_isRunning;
}
