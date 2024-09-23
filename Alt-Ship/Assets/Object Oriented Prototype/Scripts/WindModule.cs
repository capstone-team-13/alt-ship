using UnityEngine;
using UnityEngine.Events;

namespace EE.Prototype.OOP
{
    public class WindModule : MonoBehaviour
    {
        private float m_speed;
        private Vector3 m_direction;

        public Vector3 Direction => m_direction;
        public Vector3 Velocity => m_speed * m_direction;

        public UnityEvent<Vector3> OnDirectionChanged;

        private void FixedUpdate()
        {
            m_speed = Random.Range(0.0f, 3.0f);
            m_direction = Random.insideUnitSphere;
            m_direction.y = 0;

            OnDirectionChanged?.Invoke(m_direction);
        }
    }
}