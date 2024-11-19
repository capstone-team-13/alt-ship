#include <ode/ode.h>

struct CollisionInfo
{
    dWorldID world;
    dJointGroupID contactGroup;
};