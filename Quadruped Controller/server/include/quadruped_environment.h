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

    std::unique_ptr<dReal[]>
        m_result;

    dReal m_springConstant = 16;
    dReal m_dampingConstant = 6;
    Vector3f m_targetHeight{0, -2, 0};

    Vector2f m_length{2, 2};
    Vector2f m_theta{0, 0};

    Vector3f m_previousEndEffectorPosition{0, 0, 0};

    Vector3f __M__CalculateVirtualForce(Vector3f currentPosition, Vector3f currentVelocity) const;
    Vector3f __M_CalculateForwardKinematic() const;
    Matrix2f __M_MakeJacobianTransport() const;

    Vector3f __M_QuaternionToEuler(const dQuaternion quaternion)
    {
        dReal roll, pitch, yaw;

        dReal w = quaternion[0];
        dReal x = quaternion[1];
        dReal y = quaternion[2];
        dReal z = quaternion[3];

        dReal ysqr = y * y;

        dReal t0 = +2.0 * (w * x + y * z);
        dReal t1 = +1.0 - 2.0 * (x * x + ysqr);
        roll = std::atan2(t0, t1);

        dReal t2 = +2.0 * (w * y - z * x);
        t2 = t2 > 1.0 ? 1.0 : t2;
        t2 = t2 < -1.0 ? -1.0 : t2;
        pitch = std::asin(t2);

        dReal t3 = +2.0 * (w * z + x * y);
        dReal t4 = +1.0 - 2.0 * (ysqr + z * z);
        yaw = std::atan2(t3, t4);

        return Vector3f(roll, pitch, yaw);
    }

    Vector3f __M_GetEulerAnglesFromQuaternion(dBodyID body)
    {
        const dReal *quat = dBodyGetQuaternion(body);
        dQuaternion q = {quat[0], quat[1], quat[2], quat[3]};
        return __M_QuaternionToEuler(q);
    }

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
