#include <quadruped_environment.h>

QuadrupedEnvironment::QuadrupedEnvironment()
    : m_upperJointBody(nullptr), m_upperJointGeom(nullptr),
      //   m_lowerJointBody(nullptr), m_lowerJointGeom(nullptr),
      m_hingeJoint(nullptr), m_planeGeom(nullptr),
      m_result(std::make_unique<dReal[]>(14))
{
    initialize();
}

void QuadrupedEnvironment::addForce(dReal x, dReal y, dReal z)
{
    dBodyAddForce(m_upperJointBody, x, y, z);
}

const std::unique_ptr<dReal[]> &QuadrupedEnvironment::result() const
{
    return m_result;
}

void QuadrupedEnvironment::onInit()
{
    dVector3 boxDemension{1.0f, 1.0f, 1.0f};

    m_upperJointBody = dBodyCreate(this->world());
    dBodySetPosition(m_upperJointBody, 0, 11, 0);
    dQuaternion upperJointQuat;
    dQFromAxisAndAngle(upperJointQuat, 0, 0, 1, M_PI / 4);
    dBodySetQuaternion(m_upperJointBody, upperJointQuat);

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

void QuadrupedEnvironment::onSimulate()
{
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
