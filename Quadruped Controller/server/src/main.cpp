
#include "server.h"

int main()
{
    Server server;

    while (server.IsRunning())
    {
        server.Tick();
    }

    return 0;
}