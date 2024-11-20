#pragma once

#include <environment.h>

class QuadrupedEnvironment final : public Environment
{
    // Plane
    dGeomID m_planeGeom;

    dBodyID m_upperJointBody;
    dGeomID m_upperJointGeom;
    // dBodyID m_lowerJointBody;
    // dGeomID m_lowerJointGeom;

    dReal m_springConstant = 130;
    dReal m_dampingConstant = 50;
    Eigen::Vector3f m_targetHeight{0, 1, 0};

    dJointID m_hingeJoint;

    std::unique_ptr<dReal[]> m_result;

    Eigen::Vector3f __M__CalculateVirtualForce() const;

public:
    QuadrupedEnvironment();
    ~QuadrupedEnvironment() = default;

    void adjustTargetHeight();
    void addForce(Eigen::Vector3f force);
    const std::unique_ptr<dReal[]> &result() const;

protected:
    void onInit() override;
    void onSimulate(float timeStep) override;
};