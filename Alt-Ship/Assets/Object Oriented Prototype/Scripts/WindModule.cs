using UnityEngine;

namespace EE.Prototype.OOP
{
    public class WindModule : MonoBehaviour
    {
        private float m_speed;
        private Vector3 m_direction;

        public Vector3 Direction => m_direction;
        public Vector3 Velocity => m_speed * m_direction;

        private void FixedUpdate()
        {
            m_direction = Random.insideUnitSphere;
        }
    }
}
