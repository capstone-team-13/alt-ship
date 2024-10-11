using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PBA
{
    public class SingleMassSolver : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_dampingConstant;
        [SerializeField] private Vector3 m_targetHeight;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 springForce = m_springConstant * (m_targetHeight - m_rigidbody.transform.position);
            Vector3 convergenceForce = m_dampingConstant * (Vector3.zero - m_rigidbody.velocity);

            Vector3 force = springForce + convergenceForce;
            m_rigidbody.AddForce(force, ForceMode.Force);
        }
    }
}