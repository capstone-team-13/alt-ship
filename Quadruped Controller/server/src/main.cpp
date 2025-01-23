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

    while (server.isRunning() && !quit)
    {
        nextTime += TARGET_FRAME_TIME;

        server.tick();

        if (server.clientSize() > 0)
        {
            const auto &body = environment.body;
            const auto &currentState = environment.states();

            server.send(0, {EventType::FL_UPDATE, currentState[0].toString()});
            // server.send(0, {EventType::BL_UPDATE, currentState[1].toString()});
            // server.send(0, {EventType::BR_UPDATE, currentState[2].toString()});
            // server.send(0, {EventType::FR_UPDATE, currentState[3].toString()});
            // server.send(0, {EventType::END_EFFECTOR_UPDATE,
            //                 (float)currentState[0].endEffector[0], (float)currentState[0].endEffector[1], (float)currentState[0].endEffector[2],
            //                 (float)currentState[1].endEffector[0], (float)currentState[1].endEffector[1], (float)currentState[1].endEffector[2],
            //                 (float)currentState[2].endEffector[0], (float)currentState[2].endEffector[1], (float)currentState[2].endEffector[2],
            //                 (float)currentState[3].endEffector[0], (float)currentState[3].endEffector[1], (float)currentState[3].endEffector[2]});
            // server.send(0, {EventType::BODY_UPDATE,
            //                 (float)body[0][0], (float)body[0][1], (float)body[0][2],
            //                 (float)body[1][0], (float)body[1][1], (float)body[1][2], (float)body[1][3]});

            std::this_thread::sleep_until(nextTime);
        }
    }
}

int main()
{
    // Ctrl + C to terminate the server
    std::signal(SIGINT, signal_handler);

    std::thread fixedUpdateThread([&]()
                                  { fixedUpdate(); });

    // auto handle = server.addPacketReceivedCallback(
    //     [](const ENetEvent &event, uint8_t eventType, const uint8_t *data, uint32_t dataLength)
    //     {
    //         // if (eventType == EventType::ADD_FORCE)
    //         // {
    //         //     // environment.adjustTargetHeight();
    //         //     server.__M_Log("Client #", event.peer->incomingPeerID, " adjusted target height");
    //         // }
    //     });

    update();

    fixedUpdateThread.join();

    server.__M_Log("Exited.");
    return 0;
}