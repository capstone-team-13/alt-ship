using JetBrains.Annotations;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace EE.Prototype.CI
{
    public class MovementSolver : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_dampingConstant;
        [SerializeField] private Vector3 m_targetPosition;

        [UsedImplicitly]
        private void Update()
        {
            Vector3 input = __M_GetUserInput();
            m_targetPosition += input * (1.0f * Time.deltaTime);
        }


        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 springForce = m_springConstant * (m_targetPosition - m_rigidbody.transform.position);
            Vector3 convergenceForce = m_dampingConstant * (Vector3.zero - m_rigidbody.velocity);

            Vector3 force = springForce + convergenceForce;
            m_rigidbody.AddForce(force, ForceMode.Force);
        }

        [System.Diagnostics.Contracts.Pure]
        private Vector3 __M_GetUserInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var movement = new Vector3(horizontal, 0.0f, vertical);
            return movement;
        }
    }
}