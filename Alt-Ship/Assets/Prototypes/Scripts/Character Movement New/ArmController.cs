using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class ArmController : MonoBehaviour
    {
        [SerializeField] private Transform m_top;
        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Transform m_bodyRotation;

        private Vector3 m_previousPosition;
        [SerializeField] private LayerMask m_ground;

        private Vector3 m_selfToBody;

        [UsedImplicitly]
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            Vector3 start = m_top.position;
            Gizmos.DrawSphere(start, 0.1f);

            Vector3 end = start + Vector3.down * 1.0f;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, 0.1f);
        }

        [UsedImplicitly]
        private void Start()
        {
            m_selfToBody = transform.position - m_body.position;
        }

        private void FixedUpdate()
        {
            OnTick();
        }

        public void OnTick()
        {
            _M_FollowBody();
            _M_AlignWithBodyRotation();
        }

        private void _M_FollowBody()
        {
            Vector3 scale = new(0.4f, 0.25f, 0.4f);
            Vector3 scaledVelocity = Vector3.Scale(m_body.velocity, scale);

            Vector3 targetPosition = m_body.position + m_selfToBody + scaledVelocity;

            float dampingFactor = 0.1f;

            transform.position = Vector3.Lerp(transform.position, targetPosition, dampingFactor);

            m_previousPosition = transform.position;
        }

        private void _M_AlignWithBodyRotation()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, m_bodyRotation.rotation, 0.1f);
        }
    }
}