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

        private float m_theta1;
        private float m_theta2;

        private Matrix3x3 m_jacobianTransport;

        private Vector3 m_previousEndEffectorPosition;

        private void Start()
        {
            m_endEffector.position = m_previousEndEffectorPosition = __M_CalculateForwardKinematic();
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            m_theta1 = m_jointBody1.transform.eulerAngles.z * Mathf.Deg2Rad;
            m_theta2 = m_jointBody2.transform.eulerAngles.z * Mathf.Deg2Rad;

            Vector3 endEffectorPosition = __M_CalculateForwardKinematic();
            Vector3 endEffectorVelocity = (m_previousEndEffectorPosition - endEffectorPosition) / Time.fixedDeltaTime;

            Vector3 virtualForce = __M__CalculateVirtualForce(endEffectorPosition, endEffectorVelocity);

            __M_UpdateJacobianTransport();

            // Debug.Log($"Virtual Force: {virtualForce}");
            // m_body.AddForce(bodyVirtualForce, ForceMode.Force);

            Debug.DrawRay(endEffectorPosition, virtualForce.normalized, Color.blue);

            Accord.Math.Vector3 torque = m_jacobianTransport * new Accord.Math.Vector3(
                virtualForce.x, virtualForce.y, virtualForce.z
            );

            JointMotor motor = m_joint1.motor;
            float inertia = m_jointBody1.inertiaTensor.z;
            float angularAcceleration = torque.X / inertia;
            float deltaTime = Time.fixedDeltaTime;
            float deltaAngularVelocity = angularAcceleration * deltaTime;
            float currentSpeed = motor.targetVelocity;
            float newSpeed = currentSpeed + deltaAngularVelocity;
            newSpeed = Mathf.Clamp(newSpeed, -15f, 15f);
            motor.targetVelocity = newSpeed;
            m_joint1.motor = motor;

            motor = m_joint2.motor;
            inertia = m_jointBody2.inertiaTensor.z;
            angularAcceleration = torque.Y / inertia;
            deltaAngularVelocity = angularAcceleration * deltaTime;
            currentSpeed = motor.targetVelocity;
            newSpeed = currentSpeed + deltaAngularVelocity;
            newSpeed = Mathf.Clamp(newSpeed, -15f, 15f);
            motor.targetVelocity = newSpeed;
            m_joint2.motor = motor;

            m_previousEndEffectorPosition = endEffectorPosition;
            m_endEffector.position = endEffectorPosition;
        }

        [System.Diagnostics.Contracts.Pure]
        private Vector3 __M__CalculateVirtualForce(Vector3 currentPosition, Vector3 currentVelocity)
        {
            Vector3 springForce = (m_targetHeight - currentPosition) * m_springConstant;
            Vector3 convergenceForce = (Vector3.zero - currentVelocity) * m_dampingConstant;

            return springForce + convergenceForce;
        }

        [System.Diagnostics.Contracts.Pure]
        private Vector3 __M_CalculateForwardKinematic()
        {
            var localPosition = new Vector3(
                m_length1 * Mathf.Cos(m_theta1) + m_length2 * Mathf.Cos(m_theta2),
                m_length1 * Mathf.Sin(m_theta1) + m_length2 * Mathf.Sin(m_theta2),
                0
            );
            // localPosition.y *= -1;

            Vector3 worldPosition = localPosition + m_jointBody1.position;
            return worldPosition;
        }

        private void __M_UpdateJacobianTransport()
        {
            m_jacobianTransport.V00 = -m_length1 * Mathf.Sin(m_theta1);
            m_jacobianTransport.V01 = m_length1 * Mathf.Cos(m_theta1);
            m_jacobianTransport.V10 = -m_length2 * Mathf.Sin(m_theta2);
            m_jacobianTransport.V11 = m_length2 * Mathf.Cos(m_theta2);
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            m_theta1 = m_jointBody1.transform.eulerAngles.z * Mathf.Deg2Rad;
            m_theta2 = m_jointBody2.transform.eulerAngles.z * Mathf.Deg2Rad;

            var upperPosition = new Vector3(
                m_length1 * Mathf.Cos(m_theta1),
                m_length1 * Mathf.Sin(m_theta1),
                0
            );


            var lowerPosition = new Vector3(
                upperPosition.x + m_length2 * Mathf.Cos(m_theta2),
                upperPosition.y + m_length2 * Mathf.Sin(m_theta2),
                0
            );

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(upperPosition, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lowerPosition, 0.2f);
        }
    }
}