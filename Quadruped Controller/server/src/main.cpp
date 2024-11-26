#include <csignal>
#include <quadruped_environment.h>
#include <server.h>
#include <chrono>
#include <event_type.h>

bool quit = false;

constexpr double FIXED_TIMESTEP = 0.01;

constexpr auto FIXED_DELTA_TIME = std::chrono::duration<double>(FIXED_TIMESTEP);
constexpr auto TARGET_FRAME_TIME = std::chrono::duration<double>(1.0 / 60.0);

double accumulatedTime = 0.0;

Server server;
QuadrupedEnvironment environment;

void signal_handler(int32_t signal)
{
    std::cout << "Caught signal: " << signal << ", shutting down..." << std::endl;
    quit = true;
}

void fixedUpdate()
{
    while (server.isRunning() && !quit)
    {
        auto startTime = std::chrono::steady_clock::now();

        std::cout << "[Fixed Update] Running...\n";

        environment.simulate(FIXED_TIMESTEP);
        auto &result = environment.result();
        // TODO: Flexiable Serialize
        server.send((uint32_t)0,
                    {1, (float)result[0], (float)result[1], (float)result[2],
                     (float)result[3], (float)result[4], (float)result[5], (float)result[6]});
        accumulatedTime -= FIXED_TIMESTEP;

        auto elapsed = std::chrono::steady_clock::now() - startTime;
        std::this_thread::sleep_for(FIXED_DELTA_TIME - elapsed);
    }
}

void update()
{
    while (server.isRunning() && !quit)
    {
        auto start = std::chrono::steady_clock::now();

        std::cout << "[Tick] Running...\n";

        server.tick();

        std::this_thread::sleep_until(start + TARGET_FRAME_TIME);
    }
}

int main()
{
    // Ctrl + C to terminate the server
    std::signal(SIGINT, signal_handler);

    std::thread fixedUpdateThread([&]()
                                  { fixedUpdate(); });

    auto handle = server.addPacketReceivedCallback(
        [](const ENetEvent &event, uint8_t eventType, [[maybe_unused]] const uint8_t *data, [[maybe_unused]] uint32_t dataLength)
        {
            if (eventType == EventType::ADD_FORCE)
            {
                environment.adjustTargetHeight();
                server.__M_Log("Client #", event.peer->incomingPeerID, " adjusted target height\n");
            }
        });

    update();

    fixedUpdateThread.join();
    return 0;
}