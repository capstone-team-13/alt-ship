using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EE.Prototype.ShipControllers
{
    public class ShipAuthoring : MonoBehaviour
    {
        [SerializeField] private float m_speed = 100.0f;
        [SerializeField] private float3 m_direction = new (1, 0, 0);
        [SerializeField] BoxCollider m_collider;

        public class Baker : Baker<ShipAuthoring>
        {
            public override void Bake(ShipAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Ship
                {
                    Speed = authoring.m_speed,
                    Direction = authoring.m_direction,
                });
            }
        }
    }
}
