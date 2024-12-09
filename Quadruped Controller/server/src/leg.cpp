#include "leg.h"

const GaitConfig Leg::config = GaitConfig();
const Vector3f Leg::springConstant = Vector3f(20, 20, 20);
const Vector3f Leg::dampingConstant = Vector3f(5, 5, 5);
float Leg::elapsedTime = 0.0f;

Leg::Leg(size_t index)
    : hipJointBody(nullptr), hipJointGeom(nullptr), hipHingeJoint(nullptr),
      upperJointBody(nullptr), upperJointGeom(nullptr), upperHingeJoint(nullptr),
      lowerJointBody(nullptr), lowerJointGeom(nullptr), lowerHingeJoint(nullptr)
{

    // Adjust Length based on index
    const float lengthAdjustments[4] = {-0.5, -0.5, 0.5, 0.5};
    length[0] = index < 4 ? lengthAdjustments[index] : length[0];
}

void Leg::init(const dWorldID &_world, const dSpaceID &_space, const Vector3f &_offset)
{
    world = _world;
    space = _space;
    offset = _offset;

    // Hip Body
    Vector3f dimension{0.1f, 0.1f, 0.1f};
    Vector3f position = {dimension[0] / 2.0f, 0.0f, 0.0f};
    position += offset;

    __M_CreateBox(hipJointBody, hipJointGeom, dimension);
    __M_ConfigBox(hipJointBody, position, 1.0f, dimension);
    // hipHingeJoint = __M_CreateHingeJoint(hipJointBody, nullptr, anchor, {1, 0, 0});

    // Upper Body
    dimension = {2.0f, 0.5f, 0.5f};
    position = {length[1] / 2.0f, 0.0f, length[0]};
    position += offset;
    Vector3f anchor = {0, 0, length[0]};
    anchor += offset;

    __M_CreateBox(upperJointBody, upperJointGeom, dimension);
    __M_ConfigBox(upperJointBody, position, 1, dimension);
    upperHingeJoint = __M_CreateHingeJoint(upperJointBody, &hipJointBody, anchor, {0, 0, 1});

    // Lower Body
    dimension = {2.0f, 0.5f, 0.5f};
    position = {length[1] + length[2] / 2.0f, 0, length[0]};
    position += offset;
    anchor = {length[1], 0, length[0]};
    anchor += offset;

    __M_CreateBox(lowerJointBody, lowerJointGeom, dimension);
    __M_ConfigBox(lowerJointBody, position, 1, dimension);
    lowerHingeJoint = __M_CreateHingeJoint(lowerJointBody, &upperJointBody, anchor, {0, 0, 1});
}

void Leg::connectTo(dBodyID body)
{
    Vector3f anchor = {0, 0, 0};
    anchor += offset;
    hipHingeJoint = __M_CreateHingeJoint(hipJointBody, &body, anchor, {1, 0, 0});
}

void Leg::simulate(float timeStep)
{
    __M_UpdateTheta();

    auto endEffectorPosition = __M_CalculateForwardKinematic();
    endEffectorPosition += offset;
    auto endEffectorVelocity = (endEffectorPosition - previousEndEffectorPosition) / timeStep;

    auto jacobianTransport = __M_MakeJacobianTransport();

    // TODO: Gait Offset
    desiredPosition = __M__CalculateDesiredPosition();
    desiredPosition[1] += -4;
    desiredPosition += offset;

    auto virtualForce = __M__CalculateVirtualForce(endEffectorPosition, endEffectorVelocity);
    Vector3f torque = jacobianTransport * Vector3f(virtualForce[0], virtualForce[1], virtualForce[2]);
    __M_ApplyTorque(torque);

    previousEndEffectorPosition = endEffectorPosition;

    __M_UpdateState();
}

void Leg::__M_UpdateTheta()
{
    theta[0] = dJointGetHingeAngle(hipHingeJoint);
    theta[1] = dJointGetHingeAngle(upperHingeJoint);
    theta[2] = dJointGetHingeAngle(lowerHingeJoint);
}

void Leg::__M_ApplyTorque(const Vector3f &torque)
{
    dJointAddHingeTorque(hipHingeJoint, torque[0]);
    dJointAddHingeTorque(upperHingeJoint, torque[1]);
    dJointAddHingeTorque(lowerHingeJoint, torque[2]);
}

void Leg::__M_CreateBox(dBodyID &body, dGeomID &geom, const Vector3f &dimension) const
{
    assert(world && "Error: Invalid world.");
    assert(space && "Error: Invalid space.");

    body = dBodyCreate(world);
    assert(body && "Error: Failed to create body.");

    geom = dCreateBox(space, dimension[0], dimension[1], dimension[2]);
    assert(geom && "Error: Failed to create box geometry.");

    dGeomSetBody(geom, body);
}

void Leg::__M_ConfigBox(dBodyID &body, const Vector3f &position, const dReal density, const Vector3f &dimension) const
{

    assert(body && "Error: Invalid body.");

    assert(dimension[0] > 0 && dimension[1] > 0 && dimension[2] > 0 && "Error: Invalid box dimensions.");

    dBodySetPosition(body, position[0], position[1], position[2]);

    dMass mass;
    dMassSetBox(&mass, density, dimension[0], dimension[1], dimension[2]);
    dBodySetMass(body, &mass);
}

dJointID Leg::__M_CreateHingeJoint(dBodyID &from, dBodyID *to, const Vector3f &anchor, const Vector3f &axis) const
{
    assert(from && "Error: Invalid 'from' body for joint.");

    assert(!(axis[0] == 0 && axis[1] == 0 && axis[2] == 0) && "Error: Invalid hinge axis.");

    dJointID joint = dJointCreateHinge(world, nullptr);
    assert(joint && "Error: Failed to create hinge joint.");

    dJointAttach(joint, from, to ? *to : nullptr);
    dJointSetHingeAnchor(joint, anchor[0], anchor[1], anchor[2]);
    dJointSetHingeAxis(joint, axis[0], axis[1], axis[2]);

    return joint;
}

Vector3f Leg::__M__CalculateDesiredPosition()
{
    const float elapsedTime = Leg::elapsedTime;
    float sigma = elapsedTime <= config.period * config.dutyCycle
                      // Swing Phase
                      ? 2.0f * M_PI * elapsedTime / (config.dutyCycle * config.period)
                      // Stance Phase
                      : 2.0f * M_PI * (elapsedTime - config.period * config.dutyCycle) / (config.period * (1 - config.dutyCycle));

    float x, y, z;

    x = elapsedTime <= config.period * config.dutyCycle
            ? (config.endX - config.startX) * ((sigma - sin(sigma)) / (2 * M_PI)) + config.startX
            : (config.startX - config.endX) * ((sigma - sin(sigma)) / (2 * M_PI)) + config.endX;

    y = (config.liftHeight * (1 - cos(sigma)) / 2.0f) - 0.325f;
    z = 0.05;

    return Vector3f(x, y, z);
}

Vector3f Leg::__M_CalculateForwardKinematic() const
{
    auto &l1 = length[0];
    auto &l2 = length[1];
    auto &l3 = length[2];

    auto &theta1 = theta[0];
    auto &theta2 = theta[1];
    auto &theta3 = theta[2];

    float x = l2 * cos(theta2) + l3 * cos(theta2 + theta3);
    float y = -l1 * sin(theta1) + l2 * sin(theta2) * cos(theta1) +
              l3 * sin(theta2 + theta3) * cos(theta1);
    float z = l1 * cos(theta1) + l2 * sin(theta2) * sin(theta1) +
              l3 * sin(theta2 + theta3) * sin(theta1);

    // Local Position
    return Vector3f(x, y, z);
}

Matrix3f Leg::__M_MakeJacobianTransport() const
{
    Matrix3f jacobianTransport;

    auto &l1 = length[0];
    auto &l2 = length[1];
    auto &l3 = length[2];

    auto &theta1 = theta[0];
    auto &theta2 = theta[1];
    auto &theta3 = theta[2];

    jacobianTransport(0, 0) = 0;
    jacobianTransport(0, 1) = -l1 * cos(theta1) - l3 * sin(theta2 + theta3) * sin(theta1) - l2 * sin(theta1) * sin(theta2);
    jacobianTransport(0, 2) = l3 * sin(theta2 + theta3) * cos(theta1) - l1 * sin(theta1) + l2 * cos(theta1) * sin(theta2);

    jacobianTransport(1, 0) = -l3 * sin(theta2 + theta3) - l2 * sin(theta2);
    jacobianTransport(1, 1) = l3 * cos(theta2 + theta3) * cos(theta1) + l2 * cos(theta1) * cos(theta2);
    jacobianTransport(1, 2) = l3 * cos(theta2 + theta3) * sin(theta1) + l2 * cos(theta2) * sin(theta1);

    jacobianTransport(2, 0) = -l3 * sin(theta2 + theta3);
    jacobianTransport(2, 1) = l3 * cos(theta2 + theta3) * cos(theta1);
    jacobianTransport(2, 2) = l3 * cos(theta2 + theta3) * sin(theta1);

    return jacobianTransport;
}

Vector3f Leg::__M__CalculateVirtualForce(const Vector3f &currentPosition, const Vector3f &currentVelocity) const
{
    Vector3f springForce;
    Vector3f convergenceForce;

    for (size_t i = 0; i < 3; ++i)
    {
        springForce[i] = Leg::springConstant[i] * (desiredPosition[i] - currentPosition[i]);
        convergenceForce[i] = Leg::dampingConstant[i] * currentVelocity[i];
    }

    return springForce - convergenceForce;
}

void Leg::__M_UpdateState()
{
    const dReal *upperJointPosition = dBodyGetPosition(upperJointBody);
    const dReal *upperJointQuaternion = dBodyGetQuaternion(upperJointBody);

    const dReal *lowerJointPosition = dBodyGetPosition(lowerJointBody);
    const dReal *lowerJointQuaternion = dBodyGetQuaternion(lowerJointBody);

    const dReal *hipJointPosition = dBodyGetPosition(hipJointBody);
    const dReal *hipJointQuaternion = dBodyGetQuaternion(hipJointBody);

    state.update({hipJointPosition, upperJointPosition, lowerJointPosition,
                  hipJointQuaternion, upperJointQuaternion, lowerJointQuaternion});
}
