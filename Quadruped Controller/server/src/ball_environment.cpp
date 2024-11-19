#include <ball_environment.h>

BounceBallEnvironment::BounceBallEnvironment() : m_ballBody(nullptr), m_ballGeom(nullptr), m_plane(nullptr), m_result(std::make_unique<dReal[]>(3))
{
    initialize();
}

const std::unique_ptr<dReal[]> &BounceBallEnvironment::result() const
{
    return m_result;
}

void BounceBallEnvironment::onInit()
{
    m_ballBody = dBodyCreate(this->world());
    dBodySetPosition(m_ballBody, 0, 25, 0);

    dMass mass;
    dMassSetSphere(&mass, 1, 0.1);
    dBodySetMass(m_ballBody, &mass);

    m_ballGeom = dCreateSphere(this->space(), 0.5);
    dGeomSetBody(m_ballGeom, m_ballBody);

    m_plane = dCreatePlane(this->space(), 0, 1, 0, 0);
}

void BounceBallEnvironment::onSimulate()
{
    const dReal *ballPosition = dBodyGetPosition(m_ballBody);

    m_result[0] = ballPosition[0];
    m_result[1] = ballPosition[1];
    m_result[2] = ballPosition[2];
}
