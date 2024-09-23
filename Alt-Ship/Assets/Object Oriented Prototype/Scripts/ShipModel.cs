using UnityEngine;

namespace EE.Prototype.OOP
{
    public class ShipModel : MonoBehaviour
    {
        [Header("Refs.")]
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private WindModule m_windSystem;

        [SerializeField] private float m_speed;
        [SerializeField] private Vector3 m_seilDirection;

        #region Unity Callbacks

        private void FixedUpdate()
        {
            var windVelocity = CalcuateWindVelocity();
            m_rigidbody.velocity += windVelocity;
        }

        #endregion

        #region Main Methods

        public Vector3 CalcuateWindVelocity()
        {
            float windEffect = 1.0f - Vector3.Dot(m_seilDirection, m_windSystem.Direction);
            return m_windSystem.Velocity * windEffect;
        }

        #endregion
    }
}
