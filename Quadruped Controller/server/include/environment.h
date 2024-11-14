#include <ode/ode.h>
#include <iostream>
#include <memory>

class Environment
{
    dWorldID m_world;
    dSpaceID m_space;
    dJointGroupID m_contactGroup;
    dBodyID m_body;
    dGeomID m_ballGeom;
    dGeomID m_plane;

    std::unique_ptr<dReal[]> m_result;

    void __M_Initialize();
    static void nearCallback(void *data, dGeomID o1, dGeomID o2);

public:
    Environment();
    ~Environment();
    void simulate(float timeStep);
    const std::unique_ptr<dReal[]> &result() const;
};