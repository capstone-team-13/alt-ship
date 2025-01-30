using JetBrains.Annotations;
using UnityEngine;
using Boopoo.Utilities;

namespace EE.Prototype.CI
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Settings")][SerializeField] private float m_speed = 14.0f;
        [Range(0.01f, 0.99f)][SerializeField] private float m_movementThreshold = 0.01f;
        [SerializeField] private float m_speedUpTime = 0.2f;
        [SerializeField] private float m_speedDownTime = 0.1f;
        private Vector3 m_currentVelocity = Vector3.zero;

        /// <summary>
        /// desire move direction
        /// </summary>
        private Vector3 m_moveDirection = Vector3.zero;

        /// <summary>
        /// Allow automatic rotate character when moving
        /// </summary>
        [SerializeField] private bool m_allowPlayerRotation = true;

        /// <summary>
        /// character rotation speed
        /// </summary>
        [SerializeField] private float m_rotationSpeed = 5.0f;

        /// <summary>
        /// Animator controller for player
        /// </summary>
        [Header("References")]
        [SerializeField]
        private Animator m_animator = null;

        /// <summary>
        /// Player's rigidbody
        /// </summary>
        [SerializeField] private Rigidbody m_rigidbody = null;

        /// <summary>
        /// Transform controls player mesh rotation
        /// </summary>
        [SerializeField] private Transform m_rotation = null;

        [UsedImplicitly]
        private void LateUpdate()
        {
            Vector3 currentInput = InputManager.GetDirectionalInput();
            m_animator.SetFloat("inputX", currentInput.x);
            m_animator.SetFloat("inputZ", currentInput.z);

            MovePlayerRelativeToCamera();
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 input = InputManager.GetDirectionalInput();

            m_rigidbody.velocity = input.magnitude > m_movementThreshold
                ? Vector3.SmoothDamp(m_rigidbody.velocity, m_moveDirection * m_speed, ref m_currentVelocity,
                    m_speedUpTime * Time.fixedDeltaTime)
                : Vector3.SmoothDamp(m_rigidbody.velocity, Vector3.zero, ref m_currentVelocity,
                    m_speedDownTime * Time.fixedDeltaTime);
        }

        private void MovePlayerRelativeToCamera()
        {
            float verticalInput = InputManager.GetVerticalInputRaw();
            m_moveDirection = InputManager.CalculateMoveDirection(Camera.main.transform);

            // Update Character Mesh Rotation
            if (m_moveDirection == Vector3.zero || !m_allowPlayerRotation) return;

            Quaternion targetRotation = verticalInput < 0
                ? Quaternion.LookRotation(-m_moveDirection)
                : Quaternion.LookRotation(m_moveDirection);

            m_rotation.rotation = Quaternion.Slerp(
                m_rotation.rotation,
                targetRotation,
                m_rotationSpeed * Time.deltaTime
            );
        }
    }
}