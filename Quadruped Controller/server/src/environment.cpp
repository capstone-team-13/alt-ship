#include <environment.h>
#include <collision_info.h>

void Environment::initialize()
{
    dInitODE2(0);

    m_world = dWorldCreate();
    dWorldSetGravity(m_world, 0, -9.81, 0);

    m_space = dHashSpaceCreate(0);

    m_contactGroup = dJointGroupCreate(0);

    onInit();
}

void Environment::__M_HandleCollision(void *data, dGeomID geom1, dGeomID geom2)
{
    auto collisonInfo = static_cast<CollisionInfo *>(data);

    dBodyID body1 = dGeomGetBody(geom1);
    dBodyID body2 = dGeomGetBody(geom2);

    const int MAX_NUM_CONTACTS = 8;
    dContact contacts[MAX_NUM_CONTACTS];

    int numc = dCollide(geom1, geom2, MAX_NUM_CONTACTS, &contacts[0].geom, sizeof(dContact));

    for (int i = 0; i < numc; i++)
    {
        contacts[i].surface.mode = dContactSoftERP | dContactSoftCFM | dContactApprox1 |
                                   dContactSlip1 | dContactSlip2;

        contacts[i].surface.mu = 1.0;
        contacts[i].surface.soft_erp = 0.2;
        contacts[i].surface.soft_cfm = 1e-4;
        contacts[i].surface.slip1 = 0.01;
        contacts[i].surface.slip2 = 0.01;

        dJointID contact = dJointCreateContact(collisonInfo->world, collisonInfo->contactGroup, &contacts[i]);
        dJointAttach(contact, body1, body2);
    }
}

Environment::~Environment()
{
    dWorldDestroy(m_world);
    dSpaceDestroy(m_space);
    dCloseODE();
}

void Environment::simulate(float timeStep)
{
    CollisionInfo collisionInfo{m_world, m_contactGroup};

    dSpaceCollide(m_space, &collisionInfo, &Environment::__M_HandleCollision);

    dWorldStep(m_world, static_cast<dReal>(timeStep));

    dJointGroupEmpty(m_contactGroup);

    onSimulate();
}

const dWorldID &Environment::world() const
{
    return m_world;
}

const dSpaceID &Environment::space() const
{
    return m_space;
}
