#include <environment.h>
#include <memory>

class BounceBallEnvironment final : public Environment
{
    // Ball
    dBodyID m_ballBody;
    dGeomID m_ballGeom;
    // Plane
    dGeomID m_plane;

    std::unique_ptr<dReal[]> m_result;

public:
    BounceBallEnvironment();
    ~BounceBallEnvironment() = default;

    const std::unique_ptr<dReal[]> &result() const;

protected:
    void onInit() override;
    void onSimulate() override;
};