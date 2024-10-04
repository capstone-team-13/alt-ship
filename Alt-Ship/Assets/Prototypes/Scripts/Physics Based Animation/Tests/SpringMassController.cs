using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PBA
{
    public class SpringMassController : MonoBehaviour
    {
        [Header("Configs")] [SerializeField] private float m_springConstant = 1.0f;
        [Range(0, 1)] [SerializeField] private float m_dampingCoefficient = 0.2f;
        [SerializeField] private float restLength = 1.0f;
        [SerializeField] private float m_mass = 1.0f;

        [Header("Refs.")] [SerializeField] private Rigidbody m_rigidbody = null;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            m_rigidbody.mass = m_mass;

            var input = new Vector3(Input.GetAxisRaw("Horizontal") * 10, 0, Input.GetAxisRaw("Vertical") * 10);

            Vector3 displacement = m_rigidbody.position - restLength * Vector3.up;
            Vector3 springForce = -m_springConstant * displacement;
            Vector3 dampingForce = -m_dampingCoefficient * m_rigidbody.velocity;

            Vector3 force = springForce + dampingForce + input;
            m_rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, m_rigidbody.gameObject.transform.position);
        }
    }
}