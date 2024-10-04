using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace EE.Prototype.PBA
{
    [RequireComponent(typeof(Rigidbody))]
    public class HopperSimulator : MonoBehaviour
    {
        [Header("Configs")] [SerializeField] private float m_speed;
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_legRestLength;
        [SerializeField] private float m_thrustForce;

        [Header("Sensors")] [SerializeField] private GroundSensor m_groundSensor;


        private Rigidbody m_rigidbody;

        /// <summary>
        /// XZ Plane
        /// </summary>
        private float2 m_velocity;

        /// <summary>
        /// XZ Plane
        /// </summary>
        private float2 m_targetVelocity;

        private float3 m_eulerAngles;
        private float3 m_angularVelocity;

        private bool m_wasGrounded;
        private bool m_isGrounded;
        private float m_lastGroundedTime;


        #region Unity Callbacks

        [UsedImplicitly]
        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            __M_HandleInput();

            m_isGrounded = m_groundSensor.IsGrounded();

            __M_UpdateRotation();
            __M_UpdateTransform();
            __M_UpdateSpring();
            __M_UpdateJoints();

            __M_UpdateLastGroundedTime();
            m_wasGrounded = m_isGrounded;

            __M_UpdateVelocity();
            __M_UpdateState();
        }

        #endregion

        #region Internal

        private void __M_HandleInput()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            var input = new Vector2(horizontalInput, verticalInput);

            // Update velocity
            m_targetVelocity = input * (m_speed * Time.fixedDeltaTime);
        }

        private void __M_UpdateRotation()
        {
            float3 currentEulerAngles = transform.eulerAngles;
            float3 currentAngularVelocity = (currentEulerAngles - m_eulerAngles) / Time.fixedDeltaTime;

            m_eulerAngles = currentEulerAngles;
            m_angularVelocity = (m_angularVelocity + currentEulerAngles) * 0.5f;
        }

        // TODO: Transformation
        private void __M_UpdateTransform()
        {
        }

        private void __M_UpdateSpring()
        {
            float currentLength;
            float3 springVelocity;
        }

        private void __M_UpdateJoints()
        {
        }

        private void __M_UpdateVelocity()
        {
        }

        private void __M_UpdateLastGroundedTime()
        {
        }

        #region State Management

        private int m_stateIndex;

        // Flight -> Landing -> Compression -> Thrust -> Unloading
        private const int MAX_STATE_COUNT = 5;
        private System.Action[] m_onFixedUpdate;

        private void __M_UpdateState()
        {
        }

        #endregion

        #endregion
    }
}