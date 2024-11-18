#include <ode/ode.h>

class Environment
{
    dWorldID m_world;
    dSpaceID m_space;
    dJointGroupID m_contactGroup;

    // dBodyID m_upperJointBody;
    // dGeomID m_upperJointGeom;
    // dBodyID m_lowerJointBody;
    // dGeomID m_lowerJointGeom;

    static void __M_HandleCollision(void *data, dGeomID geom1, dGeomID geom2);

public:
    void simulate(float timeStep);

    const dWorldID &world() const;
    const dSpaceID &space() const;

protected:
    Environment() = default;
    virtual ~Environment();
    virtual void onInit() = 0;
    virtual void onSimulate() = 0;
    void initialize();
};