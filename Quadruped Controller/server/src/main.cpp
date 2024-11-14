
#include <csignal>
#include <iostream>
#include <server.h>

bool quit = false;

void signal_handler(int32_t signal)
{
    std::cout << "Caught signal: " << signal << ", shutting down...";
    quit = true;
}

int main()
{
    // Ctrl + C to terminate the server
    std::signal(SIGINT, signal_handler);

    Server server;

    while (server.isRunning())
    {
        server.tick();
        if (quit)
            break;
    }

    return 0;
}