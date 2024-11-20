#include <quadruped_environment.h>

Eigen::Vector3f QuadrupedEnvironment::__M__CalculateVirtualForce() const
{
    const dReal *position = dBodyGetPosition(m_upperJointBody);
    const dReal *velocity = dBodyGetLinearVel(m_upperJointBody);

    Eigen::Vector3f posEigen = Eigen::Vector3f(position[0], position[1], position[2]);
    Eigen::Vector3f velEigen = Eigen::Vector3f(velocity[0], velocity[1], velocity[2]);

    Eigen::Vector3f springForce = static_cast<float>(m_springConstant) * (m_targetHeight - posEigen);

    Eigen::Vector3f convergenceForce = m_dampingConstant * (-velEigen);

    return springForce + convergenceForce;
}

QuadrupedEnvironment::QuadrupedEnvironment()
    : m_upperJointBody(nullptr), m_upperJointGeom(nullptr),
      //   m_lowerJointBody(nullptr), m_lowerJointGeom(nullptr),
      m_hingeJoint(nullptr), m_planeGeom(nullptr),
      m_result(std::make_unique<dReal[]>(14))
{
    initialize();
}

void QuadrupedEnvironment::adjustTargetHeight()
{
    m_targetHeight[1] = m_targetHeight[1] + 1;
}

void QuadrupedEnvironment::addForce(Eigen::Vector3f force)
{
    dBodyAddForce(m_upperJointBody, force[0], force[1], force[2]);
}

const std::unique_ptr<dReal[]> &QuadrupedEnvironment::result() const
{
    return m_result;
}

void QuadrupedEnvironment::onInit()
{
    dVector3 boxDemension{1.0f, 1.0f, 1.0f};

    m_upperJointBody = dBodyCreate(this->world());
    dBodySetPosition(m_upperJointBody, 0, 1, 0);
    // dQuaternion upperJointQuat;
    // dQFromAxisAndAngle(upperJointQuat, 0, 0, 1, M_PI / 4);
    // dBodySetQuaternion(m_upperJointBody, upperJointQuat);

    dMass upperJointMass;
    dMassSetBox(&upperJointMass, 1, boxDemension[0], boxDemension[1], boxDemension[2]);
    dBodySetMass(m_upperJointBody, &upperJointMass);

    // m_lowerJointBody = dBodyCreate(this->world());
    // dBodySetPosition(m_lowerJointBody, 0, 10, 0);
    // dQuaternion lowerJointQuat;
    // dQFromAxisAndAngle(lowerJointQuat, 0, 0, 1, -M_PI / 4);
    // dBodySetQuaternion(m_lowerJointBody, lowerJointQuat);

    // dMass lowerJointMass;
    // dMassSetBox(&lowerJointMass, 1, boxDemension[0], boxDemension[1], boxDemension[2]);
    // dBodySetMass(m_lowerJointBody, &lowerJointMass);

    m_upperJointGeom = dCreateBox(this->space(), boxDemension[0], boxDemension[1], boxDemension[2]);
    dGeomSetBody(m_upperJointGeom, m_upperJointBody);

    // m_lowerJointGeom = dCreateBox(this->space(), boxDemension[0], boxDemension[1], boxDemension[2]);
    // dGeomSetBody(m_lowerJointGeom, m_lowerJointBody);

    m_planeGeom = dCreatePlane(this->space(), 0, 1, 0, 0);

    // m_hingeJoint = dJointCreateHinge(this->world(), nullptr);
    // dJointAttach(m_hingeJoint, m_upperJointBody, m_lowerJointBody);
    // dJointSetHingeAnchor(m_hingeJoint, 0, 10.5f, 0); // TODO: Anchor Variable
    // dJointSetHingeAxis(m_hingeJoint, 1, 0, 0);
}

void QuadrupedEnvironment::onSimulate(float timeStep)
{
    auto force = __M__CalculateVirtualForce();

    addForce(force);

    dBodySetAngularVel(m_upperJointBody, 0.0, 0.0, 0.0);

    const dReal *upperJointPosition = dBodyGetPosition(m_upperJointBody);
    const dReal *upperJointQuaternion = dBodyGetQuaternion(m_upperJointBody);

    // const dReal *lowerJointPosition = dBodyGetPosition(m_lowerJointBody);
    // const dReal *lowerJointQuaternion = dBodyGetQuaternion(m_lowerJointBody);

    m_result[0] = upperJointPosition[0];
    m_result[1] = upperJointPosition[1];
    m_result[2] = upperJointPosition[2];
    m_result[3] = upperJointQuaternion[0];
    m_result[4] = upperJointQuaternion[1];
    m_result[5] = upperJointQuaternion[2];
    m_result[6] = upperJointQuaternion[3];

    // m_result[7] = lowerJointPosition[0];
    // m_result[8] = lowerJointPosition[1];
    // m_result[9] = lowerJointPosition[2];
    // m_result[10] = lowerJointQuaternion[0];
    // m_result[11] = lowerJointQuaternion[1];
    // m_result[12] = lowerJointQuaternion[2];
    // m_result[13] = lowerJointQuaternion[3];
}
