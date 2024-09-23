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
            SmoothWind();
        }

        // TODO: REMOVE ME
        private float noiseOffsetX = 0.0f;
        private float noiseOffsetZ = 0.0f;
        private float noiseSpeed = 0.1f;

        private void SmoothWind()
        {
            noiseOffsetX += Time.deltaTime * noiseSpeed;
            noiseOffsetZ += Time.deltaTime * noiseSpeed;

            float windX = Mathf.PerlinNoise(noiseOffsetX, 0.0f) * 2 - 1;
            float windZ = Mathf.PerlinNoise(0.0f, noiseOffsetZ) * 2 - 1;

            m_direction = new Vector3(windX, 0, windZ).normalized;

            m_speed = Mathf.PerlinNoise(noiseOffsetX, noiseOffsetZ) * 3.0f;

            OnDirectionChanged?.Invoke(m_direction);
        }
    }
}