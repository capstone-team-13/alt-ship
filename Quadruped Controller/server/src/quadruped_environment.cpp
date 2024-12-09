#include <quadruped_environment.h>

QuadrupedEnvironment::QuadrupedEnvironment()
    : m_frontLeft(0), m_backLeft(1), m_backRight(2), m_frontRight(3),
      m_planeGeom(nullptr),
      m_states()
{
    // Base Initialize
    initialize();
}

const std::array<LegFrame, 4> &QuadrupedEnvironment::states() const
{
    return m_states;
}

const dReal *QuadrupedEnvironment::plane() const
{
    return dGeomGetPosition(m_planeGeom);
}

void QuadrupedEnvironment::onInit()
{
    const dWorldID &word = this->world();
    const dSpaceID &space = this->space();

    m_frontLeft.init(word, space, {-1.75, 0, -1.25});
    m_backLeft.init(word, space, {1.75, 0, -1.25});
    m_backRight.init(word, space, {1.75, 0, 1.25});
    m_frontRight.init(word, space, {-1.75, 0, 1.25});

    constexpr dVector3 dimension = {4, 1, 2};
    constexpr dVector3 position = {0, 0, 0};

    m_body = dBodyCreate(this->world());
    m_bodyGeom = dCreateBox(space, dimension[0], dimension[1], dimension[2]);

    dGeomSetBody(m_bodyGeom, m_body);
    dBodySetPosition(m_body, position[0], position[1], position[2]);

    dMass mass;
    dMassSetBox(&mass, 1, dimension[0], dimension[1], dimension[2]);
    dBodySetMass(m_body, &mass);

    m_frontLeft.connectTo(m_body);
    m_backLeft.connectTo(m_body);
    m_backRight.connectTo(m_body);
    m_frontRight.connectTo(m_body);

    m_planeGeom = dCreatePlane(space, 0, 1, 0, -4);
}

void QuadrupedEnvironment::onSimulate(float timeStep)
{
    Leg &currentLeg = m_frontLeft;
    currentLeg.simulate(timeStep);

    m_frontLeft.simulate(timeStep);
    m_backLeft.simulate(timeStep);
    m_backRight.simulate(timeStep);
    m_frontRight.simulate(timeStep);

    Leg::elapsedTime += timeStep;
    Leg::elapsedTime = fmod(Leg::elapsedTime, Leg::config.period);

    m_states[0] = m_frontLeft.state;
    m_states[1] = m_backLeft.state;
    m_states[2] = m_backRight.state;
    m_states[3] = m_frontRight.state;

    for (auto &state : m_states)
        state.generateBinaryStream();

    body[0] = dBodyGetPosition(m_body);
    body[1] = dBodyGetQuaternion(m_body);
}
