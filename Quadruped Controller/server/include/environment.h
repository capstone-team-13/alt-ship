#pragma once

#include <pch.h>

class Environment
{
    dWorldID m_world;
    dSpaceID m_space;
    dJointGroupID m_contactGroup;

    static void __M_HandleCollision(void *data, dGeomID geom1, dGeomID geom2);

public:
    void simulate(float timeStep);

    const dWorldID &world() const;
    const dSpaceID &space() const;

protected:
    Environment() = default;
    virtual ~Environment();
    virtual void onInit() = 0;
    virtual void onSimulate(float timeStep) = 0;
    void initialize();
};