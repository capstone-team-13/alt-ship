using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class BodyController : MonoBehaviour, INotifiable
    {
        [Header("Config")] [SerializeField] private float m_jumpForce = 20.0f;
        [SerializeField] private float m_moveSpeed = 3.0f;

        [Header("Refs.")] [SerializeField] private Rigidbody m_rigidBody;
        [SerializeField] private Transform m_hip;
        [SerializeField] private Transform m_shoulder;
        [SerializeField] private Transform m_rotateMe;

        // TODO: Refactor to frequency
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_dampingConstant;
        [SerializeField] private Vector3 m_targetPosition;

        [SerializeField] private LayerMask m_ground;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 groundOffset = _M_CalculateGroundOffset();

            Vector3 gaitOffset = _M_UpdateGaitOffset(Time.timeSinceLevelLoad);
            __M_RotateHip(Time.timeSinceLevelLoad);
            __M_RotateShoulder(Time.timeSinceLevelLoad);

            // Store target position at this frame;
            Vector3 currentTargetPosition = m_targetPosition;

            m_targetPosition.y += groundOffset.y;
            m_targetPosition += gaitOffset;

            Vector3 supportForce = _M_SolveSupportForce();
            m_rigidBody.AddForce(supportForce, ForceMode.Force);

            // _M_FaceMovingDirection();

            // Reset Target Position
            m_targetPosition = currentTargetPosition;
        }


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

        private Vector3 _M_UpdateGaitOffset(float timeSinceLastStep)
        {
            float amplitude = 0.1f;

            float frequency = 1.0f;

            float yPosition = amplitude * Mathf.Sin(Mathf.PI * frequency * timeSinceLastStep);

            return new Vector3(0, yPosition, 0);
        }

        private void __M_RotateHip(float timeSinceLastStep)
        {
            float amplitude = 15f;

            float frequency = 1.0f;

            float yRotation = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * timeSinceLastStep);

            m_hip.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        private void __M_RotateShoulder(float timeSinceLastStep)
        {
            float amplitude = -15f;

            float frequency = 1.0f;

            float yRotation = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * timeSinceLastStep);

            m_shoulder.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        private void _M_FaceMovingDirection()
        {
            Vector3 direction = m_rigidBody.velocity.normalized;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            m_rotateMe.rotation = Quaternion.Slerp(m_rotateMe.rotation, targetRotation, 0.1f);
        }

        #endregion

        public void Notify(params object[] data)
        {
            var userInput = (Vector3)data[0];
            var jumpInput = (float)data[1];

            Move(userInput);

            if (userInput != Vector3.zero) _M_FaceMovingDirection();

            if (jumpInput > 0) Jump();
        }
    }
}