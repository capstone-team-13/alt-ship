using Unity.Entities;
using Unity.Transforms;

namespace EE.Prototype.ShipControllers
{
    public partial struct ShipControllerSystem : ISystem
    {
        public readonly void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (ship, transform) in SystemAPI.Query<RefRO<Ship>, RefRW<LocalTransform>>())
            {
                var direction = ship.ValueRO.Direction;
                var speed = ship.ValueRO.Speed;

                transform.ValueRW.Position += direction * speed * deltaTime;
            }
        }
    }
}
