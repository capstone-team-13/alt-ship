#include <ODE/ode.h>

class Environment
{
    const dReal density = 1.0;
    const dReal radius = 0.3;
    const dReal starting_height = 10.0;
    const dReal gravity_y = -9.81;

public:
    Environment();
    ~Environment();
};