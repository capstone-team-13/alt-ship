#include <environment.h>
#include <memory>
#include <iostream>

class QuadrupedEnvironment final : public Environment
{
    // Plane
    dGeomID m_planeGeom;

    dBodyID m_upperJointBody;
    dGeomID m_upperJointGeom;
    dBodyID m_lowerJointBody;
    dGeomID m_lowerJointGeom;

    std::unique_ptr<dReal[]> m_result;

public:
    QuadrupedEnvironment();
    ~QuadrupedEnvironment() = default;

    const std::unique_ptr<dReal[]> &result() const;

protected:
    void onInit() override;
    void onSimulate() override;
};