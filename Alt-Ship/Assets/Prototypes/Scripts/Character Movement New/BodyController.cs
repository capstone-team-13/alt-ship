using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class BodyController : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private float m_jumpForce = 20.0f;
        [SerializeField] private float m_moveSpeed = 3.0f;

        [SerializeField] private Rigidbody m_rigidBody;

        // TODO: Refactor to frequency
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_dampingConstant;
        [SerializeField] private Vector3 m_targetPosition;

        [SerializeField] private LayerMask m_ground;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 groundOffset = _M_CalculateGroundOffset();

            Vector3 currentTargetPosition = m_targetPosition;
            m_targetPosition.y += groundOffset.y;

            Vector3 supportForce = _M_SolveSupportForce();
            m_rigidBody.AddForce(supportForce, ForceMode.Force);

            // Reset Target Position
            m_targetPosition = currentTargetPosition;
        }

        // BUG: Cause Height Increment Issue
        // [UsedImplicitly]
        // private void OnCollisionEnter(Collision collision)
        // {
        //     const float bounceBackPercentage = 0.25f;
        //     m_targetPosition += collision.relativeVelocity * bounceBackPercentage;
        // }

        [UsedImplicitly]
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_targetPosition, 0.1f);

            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = Vector3.down;
            const float rayDistance = 20.0f;

            var hitSomething = Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, rayDistance, m_ground);
            if (hitSomething) Gizmos.DrawSphere(hitInfo.point, 0.1f);
        }

        #region API

        // NOTE: Mostly used in FixedUpdate
        public void Move(Vector3 userInput)
        {
            var speed = m_moveSpeed * Time.fixedDeltaTime;
            m_targetPosition += userInput * speed;
        }

        public void Jump()
        {
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        #endregion

        #region Internal

        private Vector3 _M_CalculateGroundOffset()
        {
            var offset = new Vector3();

            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = Vector3.down;
            const float rayDistance = 20.0f;

            var hitSomething = Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, rayDistance, m_ground);

            if (hitSomething) offset = hitInfo.point;

            return offset;
        }

        private Vector3 _M_SolveSupportForce()
        {
            Vector3 springForce = m_springConstant * (m_targetPosition - m_rigidBody.transform.position);
            Vector3 convergenceForce = m_dampingConstant * (Vector3.zero - m_rigidBody.velocity);

            Vector3 force = springForce + convergenceForce;
            return force;
        }

        private void _M_SolveConstraint()
        {
        }

        #endregion
    }
}