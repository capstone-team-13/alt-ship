using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class HeadController : MonoBehaviour
    {
        // [SerializeField] private float m_beta;
        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Transform m_bodyRotation;

        private Vector3 m_previousPosition;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            _M_FollowBody();
            _M_AlignWithBodyRotation();
        }

        private void _M_FollowBody()
        {
            Vector3 targetDistanceToBody = Vector3.up * 2.75f;

            Vector3 scale = new(0.4f, 0.25f, 0.4f);
            Vector3 scaledVelocity = Vector3.Scale(m_body.velocity, scale);

            Vector3 targetPosition = m_body.position + targetDistanceToBody + scaledVelocity;

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