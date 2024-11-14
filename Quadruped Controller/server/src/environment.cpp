#include <environment.h>

void Environment::__M_Initialize()
{
    dInitODE();

    m_world = dWorldCreate();
    m_space = dHashSpaceCreate(0);
    m_contactGroup = dJointGroupCreate(0);

    dWorldSetGravity(m_world, 0, 0, -9.81);

    m_body = dBodyCreate(m_world);
    dMass mass;
    dMassSetSphere(&mass, 5, 0.1);
    mass.mass = 5.0;
    dBodySetMass(m_body, &mass);
    dBodySetPosition(m_body, 0, 0, 50);

    m_ballGeom = dCreateSphere(m_space, 0.5);
    dGeomSetBody(m_ballGeom, m_body);

    m_plane = dCreatePlane(m_space, 0, 0, 1, 0);

    std::cout << "Environment initialized with a falling ball and a ground plane." << std::endl;
}

void Environment::nearCallback(void *data, dGeomID o1, dGeomID o2)
{
    Environment *env = static_cast<Environment *>(data);

    const int MAX_CONTACTS = 20;
    dContact contact[MAX_CONTACTS];
    int numc = dCollide(o1, o2, MAX_CONTACTS, &contact[0].geom, sizeof(dContact));

    for (int i = 0; i < numc; i++)
    {
        contact[i].surface.mode = dContactBounce;
        contact[i].surface.bounce = 0.5;
        contact[i].surface.bounce_vel = 0.1;
        contact[i].surface.mu = dInfinity;

        dJointID c = dJointCreateContact(env->m_world, env->m_contactGroup, &contact[i]);
        dJointAttach(c, dGeomGetBody(contact[i].geom.g1), dGeomGetBody(contact[i].geom.g2));
    }
}

Environment::Environment() : m_world(nullptr), m_space(nullptr), m_contactGroup(nullptr), m_body(nullptr), m_plane(nullptr), m_result(std::make_unique<dReal[]>(3))
{
    __M_Initialize();
}

Environment::~Environment()
{
    dWorldDestroy(m_world);
    dSpaceDestroy(m_space);
    dJointGroupDestroy(m_contactGroup);
    dCloseODE();
}

void Environment::simulate(float timeStep)
{
    dSpaceCollide(m_space, this, &Environment::nearCallback);

    dWorldQuickStep(m_world, timeStep);

    dJointGroupEmpty(m_contactGroup);

    const dReal *pos = dBodyGetPosition(m_body);
    const dReal *vel = dBodyGetLinearVel(m_body);

    if (pos && vel)
    {
        m_result[0] = pos[0];
        m_result[1] = pos[1];
        m_result[2] = pos[2];
    }
}

const std::unique_ptr<dReal[]> &Environment::result() const
{
    return m_result;
}
