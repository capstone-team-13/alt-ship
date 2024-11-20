#include "quadruped_environment.h"

QuadrupedEnvironment::QuadrupedEnvironment()
    : m_upperJointBody(nullptr), m_upperJointGeom(nullptr),
      m_lowerJointBody(nullptr), m_lowerJointGeom(nullptr),
      m_planeGeom(nullptr), m_result(std::make_unique<dReal[]>(6))
{
    initialize();
}

const std::unique_ptr<dReal[]> &QuadrupedEnvironment::result() const
{
    return m_result;
}

void QuadrupedEnvironment::onInit()
{
    m_upperJointBody = dBodyCreate(this->world());
    dBodySetPosition(m_upperJointBody, 0, 12, 0);
    dMass upperJointMass;
    dMassSetBox(&upperJointMass, 1, 1, 1, 1);
    dBodySetMass(m_upperJointBody, &upperJointMass);

    m_lowerJointBody = dBodyCreate(this->world());
    dBodySetPosition(m_lowerJointBody, 0, 10, 0);
    dMass lowerJointMass;
    dMassSetBox(&lowerJointMass, 1, 1, 1, 1);
    dBodySetMass(m_lowerJointBody, &lowerJointMass);

    m_upperJointGeom = dCreateBox(this->space(), 1, 1, 1);
    dGeomSetBody(m_upperJointGeom, m_upperJointBody);

    m_lowerJointGeom = dCreateBox(this->space(), 1, 1, 1);
    dGeomSetBody(m_lowerJointGeom, m_lowerJointBody);

    m_planeGeom = dCreatePlane(this->space(), 0, 1, 0, 0);
}

void QuadrupedEnvironment::onSimulate()
{
    const dReal *upperJointPosition = dBodyGetPosition(m_upperJointBody);
    const dReal *lowerJointPosition = dBodyGetPosition(m_lowerJointBody);

    m_result[0] = upperJointPosition[0];
    m_result[1] = upperJointPosition[1];
    m_result[2] = upperJointPosition[2];
    m_result[3] = lowerJointPosition[0];
    m_result[4] = lowerJointPosition[1];
    m_result[5] = lowerJointPosition[2];
}
