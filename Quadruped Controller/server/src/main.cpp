#include <csignal>
#include <environment.h>
#include <iostream>
#include <server.h>
#include <chrono>
#include <thread>

bool quit = false;

constexpr double FIXED_TIMESTEP = 0.02;
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
    Environment environment;

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
            std::cout << "Position: (" << result[0] << ", " << result[1] << ", " << result[2] << ")\n";
            server.send((uint32_t)0, {1, (float)result[0], (float)result[1], (float)result[2]});
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