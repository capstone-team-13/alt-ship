#include <quadruped_environment.h>

Eigen::Vector3f QuadrupedEnvironment::__M__CalculateVirtualForce(Vector3f currentPosition, Vector3f currentVelocity) const
{
    Vector3f springForce = static_cast<float>(m_springConstant) * (m_targetHeight - currentPosition);
    Vector3f convergenceForce = m_dampingConstant * (-currentVelocity);

    return springForce + convergenceForce;
}

Eigen::Vector3f QuadrupedEnvironment::__M_CalculateForwardKinematic() const
{
    Vector3f localPosition = Eigen::Vector3f(
        m_length[0] * sin(m_theta[0]) + m_length[1] * sin(m_theta[1]),
        -(m_length[0] * cos(m_theta[0]) + m_length[1] * cos(m_theta[1])),
        0);

    const dReal *temp = dBodyGetPosition(m_upperJointBody);
    Vector3f upperJointPosition = Vector3f(temp[0], temp[1], temp[2]);

    // World Position
    // return localPosition + upperJointPosition;
    return localPosition;
}

Eigen::Matrix2f QuadrupedEnvironment::__M_MakeJacobianTransport() const
{
    Matrix2f jacobianTransport;
    jacobianTransport(0, 0) = -m_length[0] * sin(m_theta[0]);
    jacobianTransport(0, 1) = m_length[0] * cos(m_theta[0]);
    jacobianTransport(1, 0) = -m_length[1] * sin(m_theta[1]);
    jacobianTransport(1, 1) = m_length[1] * cos(m_theta[1]);
    return jacobianTransport;
}

QuadrupedEnvironment::QuadrupedEnvironment()
    : m_upperJointBody(nullptr), m_upperJointGeom(nullptr), m_upperHingeJoint(nullptr),
      m_lowerJointBody(nullptr), m_lowerJointGeom(nullptr), m_lowerHingeJoint(nullptr),
      m_planeGeom(nullptr),
      m_result(std::make_unique<dReal[]>(17))
{
    initialize();
}

void QuadrupedEnvironment::adjustTargetHeight()
{
    m_targetHeight[1] = m_targetHeight[1] + 1;
}

void QuadrupedEnvironment::addForce(Vector3f force)
{
    dBodyAddForce(m_upperJointBody, force[0], force[1], force[2]);
}

const std::unique_ptr<dReal[]> &QuadrupedEnvironment::result() const
{
    return m_result;
}

void QuadrupedEnvironment::onInit()
{
    constexpr dVector3 boxDemension{0.5f, 2.0f, 0.5f};

    // Upper Body
    m_upperJointBody = dBodyCreate(this->world());
    dBodySetPosition(m_upperJointBody, 0, -1, 0);
    dMass upperJointMass;
    dMassSetBox(&upperJointMass, 1, boxDemension[0], boxDemension[1], boxDemension[2]);
    dBodySetMass(m_upperJointBody, &upperJointMass);
    m_upperJointGeom = dCreateBox(this->space(), boxDemension[0], boxDemension[1], boxDemension[2]);
    dGeomSetBody(m_upperJointGeom, m_upperJointBody);

    dBodyID originBody = dBodyCreate(this->world());
    dMass originMass;
    dMassSetZero(&originMass);
    dMassSetSphereTotal(&originMass, 1, 1);
    dBodySetMass(originBody, &originMass);
    dBodySetPosition(originBody, 0, 0, 0);

    m_upperHingeJoint = dJointCreateHinge(this->world(), nullptr);
    dJointAttach(m_upperHingeJoint, m_upperJointBody, originBody);
    dJointSetHingeAnchor(m_upperHingeJoint, 0, 0, 0);
    dJointSetHingeAxis(m_upperHingeJoint, 0, 0, 1);

    // Lower Body
    m_lowerJointBody = dBodyCreate(this->world());
    dBodySetPosition(m_lowerJointBody, 0, -3, 0);
    dMass lowerJointMass;
    dMassSetBox(&lowerJointMass, 1, boxDemension[0], boxDemension[1], boxDemension[2]);
    dBodySetMass(m_lowerJointBody, &lowerJointMass);
    m_lowerJointGeom = dCreateBox(this->space(), boxDemension[0], boxDemension[1], boxDemension[2]);
    dGeomSetBody(m_lowerJointGeom, m_lowerJointBody);

    m_lowerHingeJoint = dJointCreateHinge(this->world(), nullptr);
    dJointAttach(m_lowerHingeJoint, m_lowerJointBody, m_upperJointBody);
    dJointSetHingeAnchor(m_lowerHingeJoint, 0, -2, 0);
    dJointSetHingeAxis(m_lowerHingeJoint, 0, 0, 1);

    float upperTorque = 100.0f;
    // float lowerTorque = -5.0f;
    dJointAddHingeTorque(m_upperHingeJoint, upperTorque);
    // dJointAddHingeTorque(m_lowerHingeJoint, lowerTorque);
}

void QuadrupedEnvironment::onSimulate(float timeStep)
{
    const dReal *rot1 = dBodyGetRotation(m_upperJointBody);
    m_theta[0] = atan2(rot1[7], rot1[8]);
    const dReal *rot2 = dBodyGetRotation(m_lowerJointBody);
    m_theta[1] = atan2(rot2[7], rot2[8]);

    auto endEffectorPosition = __M_CalculateForwardKinematic();
    // auto endEffectorVelocity = (m_previousEndEffectorPosition - endEffectorPosition) / timeStep;

    // auto virtualForce = __M__CalculateVirtualForce(endEffectorPosition, endEffectorVelocity);
    // auto jacobianTransport = __M_MakeJacobianTransport();

    // Vector2f torque = jacobianTransport * Vector2f(virtualForce[0], virtualForce[1]);

    // dJointAddHingeTorque(m_upperHingeJoint, torque[0]);
    // dJointAddHingeTorque(m_lowerHingeJoint, torque[1]);

    m_previousEndEffectorPosition = endEffectorPosition;

    const dReal *upperJointPosition = dBodyGetPosition(m_upperJointBody);
    const dReal *upperJointQuaternion = dBodyGetQuaternion(m_upperJointBody);

    const dReal *lowerJointPosition = dBodyGetPosition(m_lowerJointBody);
    const dReal *lowerJointQuaternion = dBodyGetQuaternion(m_lowerJointBody);

    m_result[0] = upperJointPosition[0];
    m_result[1] = upperJointPosition[1];
    m_result[2] = upperJointPosition[2];
    m_result[3] = upperJointQuaternion[0];
    m_result[4] = upperJointQuaternion[1];
    m_result[5] = upperJointQuaternion[2];
    m_result[6] = upperJointQuaternion[3];

    m_result[7] = lowerJointPosition[0];
    m_result[8] = lowerJointPosition[1];
    m_result[9] = lowerJointPosition[2];
    m_result[10] = lowerJointQuaternion[0];
    m_result[11] = lowerJointQuaternion[1];
    m_result[12] = lowerJointQuaternion[2];
    m_result[13] = lowerJointQuaternion[3];

    m_result[14] = endEffectorPosition[0];
    m_result[15] = endEffectorPosition[1];
    m_result[16] = endEffectorPosition[2];
}
