#include <csignal>
#include <quadruped_environment.h>
#include <iostream>
#include <server.h>
#include <chrono>
#include <thread>
#include <event_type.h>

bool quit = false;

constexpr double FIXED_TIMESTEP = 0.01;
constexpr double TARGET_FRAME_TIME = 1.0 / 60.0;
double accumulatedTime = 0.0;

void signal_handler(int32_t signal)
{
    std::cout << "Caught signal: " << signal << ", shutting down..." << std::endl;
    quit = true;
}

int main()
{
    // Ctrl + C to terminate the server
    std::signal(SIGINT, signal_handler);

    Server server;
    QuadrupedEnvironment environment;

    auto handle = server.addPacketReceivedCallback(
        [&environment](const ENetEvent &event, uint8_t eventType, const uint8_t *data, uint32_t dataLength)
        {
            if (eventType == EventType::ADD_FORCE)
            {
                // environment.addForce(0, 2, 0);
                environment.addForce(10, 0, 0);
                std::cout << "Client #" << event.peer->incomingPeerID << " added force\n";
            }
        });

    auto lastTime = std::chrono::high_resolution_clock::now();
    while (server.isRunning() && !quit)
    {
        auto currentTime = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> deltaTime = currentTime - lastTime;
        lastTime = currentTime;

        accumulatedTime += deltaTime.count();

        server.tick();

        while (accumulatedTime >= FIXED_TIMESTEP)
        {
            environment.simulate(FIXED_TIMESTEP);
            auto &result = environment.result();
            // TODO: Flexiable Serialize
            // server.send((uint32_t)0,
            //             {1, (float)result[0], (float)result[1], (float)result[2],
            //              (float)result[3], (float)result[4], (float)result[5], (float)result[6],
            //              (float)result[7], (float)result[8], (float)result[9],
            //              (float)result[10], (float)result[11], (float)result[12], (float)result[13]});
            server.send((uint32_t)0,
                        {1, (float)result[0], (float)result[1], (float)result[2],
                         (float)result[3], (float)result[4], (float)result[5], (float)result[6]});
            accumulatedTime -= FIXED_TIMESTEP;
        }

        auto frameEndTime = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> frameDuration = frameEndTime - currentTime;

        double sleepTime = TARGET_FRAME_TIME - frameDuration.count();
        if (sleepTime > 0)
        {
            std::this_thread::sleep_for(std::chrono::duration<double>(sleepTime));
        }
    }

    return 0;
}