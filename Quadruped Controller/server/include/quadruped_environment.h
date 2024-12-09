#pragma once
#include <environment.h>
#include <leg.h>

class QuadrupedEnvironment final : public Environment
{

    dBodyID m_body;
    dGeomID m_bodyGeom;

    Leg m_frontLeft;
    Leg m_backLeft;
    Leg m_backRight;
    Leg m_frontRight;

    // Plane
    dGeomID m_planeGeom;

    LegFrame m_states;

public:
    QuadrupedEnvironment();
    ~QuadrupedEnvironment() = default;

    const LegFrame &states() const;

    const dReal *plane() const;

    const dReal *body[2];

protected:
    void onInit() override;
    void onSimulate(float timeStep) override;
};
