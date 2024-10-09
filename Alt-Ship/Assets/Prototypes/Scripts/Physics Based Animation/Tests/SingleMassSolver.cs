using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PBA
{
    public class SingleMassSolver : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_timeConstant;
        [SerializeField] private Vector3 m_targetHeight;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            // Spring Component = Kx
            var springForce = m_springConstant * (m_targetHeight - transform.position);
            var antiGravityForce = m_timeConstant * (Vector3.zero - m_rigidbody.velocity);

            var force = springForce + antiGravityForce;
            m_rigidbody.AddForce(force, ForceMode.Force);
        }
    }
}