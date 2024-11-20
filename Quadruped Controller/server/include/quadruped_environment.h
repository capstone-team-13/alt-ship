#include <environment.h>
#include <memory>
#include <iostream>

class QuadrupedEnvironment final : public Environment
{
    // Plane
    dGeomID m_planeGeom;

    dBodyID m_upperJointBody;
    dGeomID m_upperJointGeom;
    // dBodyID m_lowerJointBody;
    // dGeomID m_lowerJointGeom;

    dReal m_springConstant = 5000;
    dReal m_dampingConstant = 150;
    dVector3 m_targetHeight{0, 4, 0};

    dJointID m_hingeJoint;

    std::unique_ptr<dReal[]> m_result;

    // dVector3 QuadrupedEnvironment::__M__CalculateVirtualForce() const;

public:
    QuadrupedEnvironment();
    ~QuadrupedEnvironment() = default;

    void addForce(dReal x, dReal y, dReal z);
    const std::unique_ptr<dReal[]> &result() const;

protected:
    void onInit() override;
    void onSimulate() override;
};