using Unity.Entities;
using Unity.Mathematics;

namespace EE.Prototype.ShipControllers
{
    public struct Ship : IComponentData
    {
        public float Speed;
        public float3 Direction;
    }
}
