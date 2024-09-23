using UnityEngine;

namespace EE.Prototype.OOP
{
    public class ShipModel : MonoBehaviour
    {
        [Header("Refs.")]
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private WindModule m_windModule;

        [SerializeField] private float m_speed;
        [SerializeField] private Vector3 m_seilDirection;

        private float m_windEffect;
        public float WindEffect => m_windEffect;

        #region Unity Callbacks

        private void FixedUpdate()
        {
            var windVelocity = CalcuateWindVelocity();
            var selfVelocity = m_speed * m_seilDirection;
            m_rigidbody.velocity = selfVelocity + windVelocity;
        }

        #endregion

        #region Main Methods

        public Vector3 CalcuateWindVelocity()
        {
            float angle = Vector3.Angle(m_seilDirection, m_windModule.Direction);

            // Calculate wind effect based on how perpendicular the sail is to the wind
            m_windEffect = Mathf.Sin(angle * Mathf.Deg2Rad);

            // Use dot product to determine the direction of the wind (positive or negative)
            // If dot product is negative, the wind is coming from behind (angle close to 180 degrees)
            float windDirection = Mathf.Sign(Vector3.Dot(m_seilDirection, m_windModule.Direction));

            // Apply wind effect and reverse direction if necessary
            return m_windModule.Velocity * m_windEffect * windDirection;
        }

        #endregion
    }
}
