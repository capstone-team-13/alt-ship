#include <environment.h>
#include <collision_info.h>

void Environment::__M_Initialize()
{
    dInitODE2(0);

    m_world = dWorldCreate();
    dWorldSetGravity(m_world, 0, -9.81, 0);

    m_space = dHashSpaceCreate(0);

    m_contactGroup = dJointGroupCreate(0);

    m_body = dBodyCreate(m_world);
    dBodySetPosition(m_body, 0, 25, 0);

    dMass mass;
    dMassSetSphere(&mass, 1, 0.1);
    dBodySetMass(m_body, &mass);

    m_ballGeom = dCreateSphere(m_space, 0.5);
    dGeomSetBody(m_ballGeom, m_body);

    m_plane = dCreatePlane(m_space, 0, 1, 0, 0);

    std::cout << "Environment initialized with a falling ball and a ground plane." << std::endl;
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

        contacts[i].surface.mu = 50.0;
        contacts[i].surface.soft_erp = 0.96;
        contacts[i].surface.soft_cfm = 2.00;

        dJointID contact = dJointCreateContact(collisonInfo->world, collisonInfo->contactGroup, &contacts[i]);
        dJointAttach(contact, body1, body2);
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
    dCloseODE();
}

void Environment::simulate(float timeStep)
{
    CollisionInfo collisionInfo{m_world, m_contactGroup};

    dSpaceCollide(m_space, &collisionInfo, &Environment::__M_HandleCollision);

    dWorldStep(m_world, static_cast<dReal>(timeStep));

    dJointGroupEmpty(m_contactGroup);

    const dReal *pos = dBodyGetPosition(m_body);

    m_result[0] = pos[0];
    m_result[1] = pos[1];
    m_result[2] = pos[2];
}

const std::unique_ptr<dReal[]> &Environment::result() const
{
    return m_result;
}
