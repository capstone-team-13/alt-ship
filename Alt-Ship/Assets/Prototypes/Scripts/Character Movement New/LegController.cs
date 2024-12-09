using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class LegController : MonoBehaviour
    {
        [SerializeField] private Transform m_top;
        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Transform m_bodyRotation;

        private Vector3 m_previousPosition;
        private Vector3 m_previousError;
        [SerializeField] private LayerMask m_ground;

        private Vector3 m_selfToBody;

        [UsedImplicitly]
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            var start = m_top.position;

            Gizmos.DrawSphere(start, 0.1f);

            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = Vector3.down;
            const float rayDistance = 20.0f;

            var hitSomething = Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, rayDistance, m_ground);
            if (hitSomething)
            {
                Vector3 hitPoint = hitInfo.point;

                float legLength = (start - hitPoint).magnitude;

                const float maxLegLength = 1.75f;

                Vector3 hitPointToStart = (hitPoint - start);
                Vector3 clampedHitPoint = Vector3.ClampMagnitude(hitPointToStart, maxLegLength) + start;
                Vector3 drawPoint = legLength > maxLegLength ? clampedHitPoint : hitPoint;

                Gizmos.DrawLine(start, drawPoint);
                Gizmos.DrawSphere(drawPoint, 0.1f);
            }
        }

        [UsedImplicitly]
        private void Start()
        {
            m_selfToBody = transform.position - m_body.position;
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