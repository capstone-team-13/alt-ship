using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class HeadController : MonoBehaviour, INotifiable
    {
        // [SerializeField] private float m_beta;
        [SerializeField] private float m_targetY = 2.75f;

        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Transform m_bodyRotation;

        [SerializeField] private float m_dampingFactor = 0.1f;

        [SerializeField] private Vector3 m_velocityScale = Vector3.zero;


        private Vector3 m_previousPosition;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            _M_FollowBody();
            _M_AlignWithBodyRotation();
        }

        private void _M_FollowBody()
        {
            Vector3 targetDistanceToBody = Vector3.up * m_targetY;

            Vector3 scaledVelocity = Vector3.Scale(m_body.velocity, m_velocityScale);

            Vector3 targetPosition = m_body.position + targetDistanceToBody + scaledVelocity;

            transform.position = Vector3.Lerp(transform.position, targetPosition, m_dampingFactor);

            m_previousPosition = transform.position;
        }

        private void _M_AlignWithBodyRotation()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, m_bodyRotation.rotation, 0.1f);
        }


        public void Notify(params object[] data)
        {
            var userInput = (Vector3)data[0];
            if (userInput != Vector3.zero) _M_FaceMovingDirection();
        }

        private void _M_FaceMovingDirection()
        {
            Vector3 direction = m_body.velocity.normalized;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
        }
    }
}