using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EE.Prototype.ShipControllers
{
    public class PlayerOneAuthoring : MonoBehaviour
    {
        [Header("Ship Model")]
        [SerializeField] private float m_speed = 100.0f;
        [SerializeField] private float3 m_direction = new(1, 0, 0);

        [Header("Camera")]
        [SerializeField] GameObject m_camera;

        public class Baker : Baker<PlayerOneAuthoring>
        {
            public override void Bake(PlayerOneAuthoring authoring)
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
