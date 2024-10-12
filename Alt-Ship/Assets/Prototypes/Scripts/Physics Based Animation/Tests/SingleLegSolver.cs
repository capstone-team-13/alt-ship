using JetBrains.Annotations;
using UnityEngine;
using Matrix3x3 = Accord.Math.Matrix3x3;

namespace EE.Prototype.PBA
{
    public class SingleLegSolver : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Rigidbody m_jointBody1;
        [SerializeField] private Rigidbody m_jointBody2;
        [SerializeField] private HingeJoint m_joint1;
        [SerializeField] private HingeJoint m_joint2;
        [SerializeField] private float m_length1;
        [SerializeField] private float m_length2;
        [SerializeField] private Transform m_endEffector;
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_dampingConstant;
        [SerializeField] private Vector3 m_targetHeight;

        private Vector3 m_virtualForce;
        private Vector3 m_previousToePosition;
        private Vector3 m_toePosition;
        private Vector3 m_toeVelocity;

        private float m_theta1;
        private float m_theta2;

        private Matrix3x3 m_jacobianTransport;

        [UsedImplicitly]
        private void Awake()
        {
            m_previousToePosition = m_toePosition = m_endEffector.position;
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            m_theta1 = m_jointBody1.transform.eulerAngles.z * Mathf.Deg2Rad;
            m_theta2 = m_jointBody2.transform.eulerAngles.z * Mathf.Deg2Rad;

            __M_CalculateForwardKinematic();
            m_toeVelocity = (m_previousToePosition - m_toePosition) / Time.fixedDeltaTime;
            __M__CalculateVirtualForce(m_body.position, m_body.velocity);
            // __M__CalculateVirtualForce(m_toePosition, m_toeVelocity);

            m_endEffector.position = m_toePosition;

            __M_CalculateJacobianTransport();

            Debug.Log($"Virtual Force: {m_virtualForce}");
            // m_body.AddForce(m_virtualForce, ForceMode.Force);

            Accord.Math.Vector3 result = m_jacobianTransport * new Accord.Math.Vector3(
                m_virtualForce.x, m_virtualForce.y, m_virtualForce.z
            );

            Debug.Log($"Torques: {result}");
            
            m_jointBody1.AddTorque(0, 0, result.X);
            m_jointBody2.AddTorque(0, 0, result.Y);
        }

        private void __M__CalculateVirtualForce(Vector3 currentPosition, Vector3 currentVelocity)
        {
            Vector3 springForce = (m_targetHeight - currentPosition) * m_springConstant;
            Vector3 convergenceForce = (Vector3.zero - currentVelocity) * m_dampingConstant;

            m_virtualForce = springForce + convergenceForce;
        }

        private void __M_CalculateForwardKinematic()
        {
            var localPosition = new Vector3(
                m_length1 * Mathf.Sin(m_theta1) + m_length2 * Mathf.Sin(m_theta2),
                m_length1 * Mathf.Cos(m_theta1) + m_length2 * Mathf.Cos(m_theta2),
                0
            );
            localPosition.y *= -1;

            Vector3 worldPosition = localPosition + m_jointBody1.position;
            m_toePosition = worldPosition;
        }

        private void __M_CalculateJacobianTransport()
        {
            m_jacobianTransport.V00 = m_length1 * Mathf.Cos(m_theta1);
            m_jacobianTransport.V01 = -m_length1 * Mathf.Sin(m_theta1);
            m_jacobianTransport.V10 = m_length2 * Mathf.Cos(m_theta2);
            m_jacobianTransport.V11 = -m_length2 * Mathf.Sin(m_theta2);
        }
    }
}