#include <csignal>
#include <quadruped_environment.h>
#include <server.h>
#include <chrono>
#include <event_type.h>

std::atomic<bool> quit = false;

constexpr double FIXED_TIMESTEP = 0.01;

constexpr auto FIXED_DELTA_TIME = std::chrono::duration_cast<std::chrono::steady_clock::duration>(std::chrono::duration<double>(FIXED_TIMESTEP));
constexpr auto TARGET_FRAME_TIME = std::chrono::duration_cast<std::chrono::steady_clock::duration>(std::chrono::duration<double>(1.0 / 60.0));

Server server;
QuadrupedEnvironment environment;

void signal_handler(int32_t signal)
{
    std::cout << "Caught signal: " << signal << ", shutting down..." << std::endl;
    quit = true;
}

void fixedUpdate()
{
    auto nextTime = std::chrono::steady_clock::now();
    while (server.isRunning() && !quit)
    {
        nextTime += FIXED_DELTA_TIME;

        if (server.clientSize() > 0)
            environment.simulate(FIXED_TIMESTEP);

        std::this_thread::sleep_until(nextTime);
    }
}

void update()
{
    auto nextTime = std::chrono::steady_clock::now();
    bool hasChanged = true;

    std::vector<float> previousResult(7, 0.0f);
    constexpr float threshold = 0.01f;

    while (server.isRunning() && !quit)
    {
        nextTime += TARGET_FRAME_TIME;

        server.tick();

        if (server.clientSize() > 0)
        {
            auto &currentResult = environment.result();

            hasChanged = false;
            for (size_t i = 0; i < previousResult.size(); ++i)
            {
                if (std::fabs(currentResult[i] - previousResult[i]) > threshold)
                {
                    hasChanged = true;
                    break;
                }
            }

            if (hasChanged)
            {
                server.send(0, {EventType::POSITION_UPDATE, (float)currentResult[0], (float)currentResult[1], (float)currentResult[2],
                                (float)currentResult[3], (float)currentResult[4], (float)currentResult[5], (float)currentResult[6]});
                server.__M_Log("Sent ", (float)currentResult[0], ", ", (float)currentResult[1], ", ", (float)currentResult[2]);

                for (size_t i = 0; i < previousResult.size(); ++i)
                    previousResult[i] = currentResult[i];
            }
        }

        std::this_thread::sleep_until(nextTime);
    }
}

int main()
{
    // Ctrl + C to terminate the server
    std::signal(SIGINT, signal_handler);

    std::thread fixedUpdateThread([&]()
                                  { fixedUpdate(); });

    auto handle = server.addPacketReceivedCallback(
        [](const ENetEvent &event, uint8_t eventType, const uint8_t *data, uint32_t dataLength)
        {
            if (eventType == EventType::ADD_FORCE)
            {
                environment.adjustTargetHeight();
                server.__M_Log("Client #", event.peer->incomingPeerID, " adjusted target height\n");
            }
        });

    update();

    fixedUpdateThread.join();

    server.__M_Log("Exited.");
    return 0;
}