#pragma once
#include <pch.h>

using Vector3f = Eigen::Vector3f;
using Matrix3f = Eigen::Matrix3f;

struct GaitConfig
{
    const float multiplier = 1.75f;
    const float period = 0.512f * multiplier;
    const float dutyCycle = 0.5f * multiplier;
    const float startX = -1.0f * multiplier;
    const float endX = 1.0f * multiplier;
    const float liftHeight = 4.0f;

    GaitConfig() = default;
};

// TODO: Template dReal
struct LegFrame
{
    struct FieldInfo
    {
        const dReal *data;
        size_t count;
    };

    const dReal *infos[6];

    std::vector<uint8_t> binaryStream;

    LegFrame()
    {
        size_t totalSize = 0;
        for (const auto &field : __M_GetFieldInfos())
            totalSize += field.count * sizeof(dReal);
        binaryStream.reserve(totalSize);
    }

    // Generate binary stream and cache it
    void generateBinaryStream()
    {
        binaryStream.clear();

        for (const auto &field : __M_GetFieldInfos())
            __M_WriteArray(binaryStream, field.data, field.count);
    }

    std::string toString() const
    {
        return std::string(binaryStream.begin(), binaryStream.end());
    }

    void update(const std::initializer_list<const dReal *> args)
    {
        size_t count = args.size();
        if (count > 6)
        {
            std::cerr << "Error: Too many inputs, maximum allowed is 6." << std::endl;
            return;
        }

        size_t i = 0;
        for (auto ptr : args)
        {
            infos[i++] = ptr;
        }

        for (; i < 6; ++i)
        {
            infos[i] = nullptr;
        }
    }

private:
    std::vector<FieldInfo> __M_GetFieldInfos() const
    {
        return {
            // Positions - Hip, Thigh, Shank
            {infos[0], 3},
            {infos[1], 3},
            {infos[2], 3},
            // Quaternions - Hip, Thigh, Shank
            {infos[3], 4},
            {infos[4], 4},
            {infos[5], 4},
        };
    }

    // Helper function to write an array to a binary stream
    void __M_WriteArray(std::vector<uint8_t> &binaryStream, const dReal *array, size_t count) const
    {
        for (size_t i = 0; i < count; ++i)
        {
            uint8_t buffer[sizeof(dReal)];
            std::memcpy(buffer, &array[i], sizeof(dReal));
            binaryStream.insert(binaryStream.end(), buffer, buffer + sizeof(dReal));
        }
    }
};

struct Leg
{
    const static GaitConfig config;
    const static Vector3f springConstant;
    const static Vector3f dampingConstant;
    static float elapsedTime;

    // Hip
    dBodyID hipJointBody;
    dGeomID hipJointGeom;
    dJointID hipHingeJoint;

    // Upper Joint
    dBodyID upperJointBody;
    dGeomID upperJointGeom;
    dJointID upperHingeJoint;

    // Lower Joint
    dBodyID lowerJointBody;
    dGeomID lowerJointGeom;
    dJointID lowerHingeJoint;

    Vector3f length{-0.5f, 2, 2};
    Vector3f theta{0.0f, 0.0f, 0.0f};

    Vector3f desiredPosition{0, 0, 0};

    Vector3f previousEndEffectorPosition{0, 0, 0};

    dWorldID world;
    dSpaceID space;
    Vector3f offset;

    size_t index;

    LegFrame state;

    // frontLeft - 0, backLeft - 1, backRight - 2, frontRight - 3,
    Leg(size_t index);

    void init(const dWorldID &_world, const dSpaceID &_space, const Vector3f &offset);
    void connectTo(dBodyID body);
    void simulate(float timeStep);

private:
    void __M_CreateBox(dBodyID &body, dGeomID &geom, const Vector3f &dimension) const;
    void __M_ConfigBox(dBodyID &body, const Vector3f &position, const dReal density, const Vector3f &dimension) const;
    dJointID __M_CreateHingeJoint(dBodyID &from, dBodyID *to, const Vector3f &anchor, const Vector3f &axis) const;

    void __M_UpdateTheta();
    void __M_ApplyTorque(const Vector3f &torque);

    Vector3f __M__CalculateDesiredPosition();
    Vector3f __M_CalculateForwardKinematic() const;
    Matrix3f __M_MakeJacobianTransport() const;
    Vector3f __M__CalculateVirtualForce(const Vector3f &currentPosition, const Vector3f &currentVelocity) const;

    void __M_UpdateState();
};