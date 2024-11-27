#pragma once

#include <environment.h>

class QuadrupedEnvironment final : public Environment
{
    using Vector2f = Eigen::Vector2f;
    using Vector3f = Eigen::Vector3f;
    using Matrix2f = Eigen::Matrix2f;

    dBodyID m_upperJointBody;
    dGeomID m_upperJointGeom;
    dJointID m_upperHingeJoint;

    dBodyID m_lowerJointBody;
    dGeomID m_lowerJointGeom;
    dJointID m_lowerHingeJoint;

    // Plane
    dGeomID m_planeGeom;

    std::unique_ptr<dReal[]> m_result;

    dReal m_springConstant = 1000;
    dReal m_dampingConstant = 50;
    Vector3f m_targetHeight{0, 3, 0};

    Vector2f m_length{2, 2};
    Vector2f m_theta{0, 0};

    Vector3f m_previousEndEffectorPosition{0, 0, 0};

    Vector3f __M__CalculateVirtualForce(Vector3f currentPosition, Vector3f currentVelocity) const;
    Vector3f __M_CalculateForwardKinematic() const;
    Matrix2f __M_MakeJacobianTransport() const;

public:
    QuadrupedEnvironment();
    ~QuadrupedEnvironment() = default;

    void adjustTargetHeight();
    void addForce(Vector3f force);
    const std::unique_ptr<dReal[]> &result() const;

protected:
    void onInit() override;
    void onSimulate(float timeStep) override;
};