using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PBA
{
    public class SingleMassSolver : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_timeConstant;
        [SerializeField] private float m_targetHeight;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            // Spring Component = Kx
            var springForce = m_springConstant * (m_targetHeight - transform.position.y);
            var antiGravityForce = m_timeConstant * (0 - m_rigidbody.velocity.y);

            Debug.Log(m_rigidbody.velocity);

            var force = springForce + antiGravityForce;
            m_rigidbody.AddForce(Vector3.up * force, ForceMode.Force);
        }
    }
}