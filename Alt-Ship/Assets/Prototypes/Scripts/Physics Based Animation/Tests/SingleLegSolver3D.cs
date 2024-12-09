using JetBrains.Annotations;
using UnityEngine;
using Matrix3x3 = Accord.Math.Matrix3x3;
using static UnityEngine.Mathf;
using UnityEngine.Windows;

namespace EE.Prototype.PBA
{
    public class SingleLegSolver3D : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Rigidbody m_jointBody1;
        [SerializeField] private Rigidbody m_jointBody2;
        [SerializeField] private float m_length1;
        [SerializeField] private float m_length2;
        [SerializeField] private float m_length3;
        [SerializeField] private Transform m_endEffector;

        private float m_theta1;
        private float m_theta2;
        private float m_theta3;

        float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        [UsedImplicitly]
        private void Start()
        {
            // m_theta1 = m_body.transform.eulerAngles.x * Mathf.Deg2Rad;
            // m_theta2 = m_jointBody1.transform.eulerAngles.z * Mathf.Deg2Rad;
            // m_theta3 = m_jointBody2.transform.eulerAngles.z * Mathf.Deg2Rad - m_theta2;

            // m_theta1 = m_body.transform.localRotation.eulerAngles.x * Mathf.Deg2Rad;
            // m_theta2 = m_jointBody1.transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
            // m_theta3 = m_jointBody2.transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;

            float l1 = 0.5f;
            float l2 = 2.0f;
            float l3 = 2.0f;

            float theta1 = 0.0f * Deg2Rad;
            float theta2 = 0.0f * Deg2Rad;
            float theta3 = 0.0f * Deg2Rad;

            Vector3 endEffectorPosition = __M_CalculateForwardKinematic(l1, l2, l3, theta1, theta2, theta3);
            Debug.Log($"End Effector Position: {endEffectorPosition}");

            theta1 = 90.0f * Deg2Rad;
            theta2 = 0.0f * Deg2Rad;
            theta3 = 0.0f;

            endEffectorPosition = __M_CalculateForwardKinematic(l1, l2, l3, theta1, theta2, theta3);
            Debug.Log($"End Effector Position: {endEffectorPosition}");

            theta1 = 180.0f * Deg2Rad;
            theta2 = 0.0f * Deg2Rad;
            theta3 = 0.0f * Deg2Rad;

            endEffectorPosition = __M_CalculateForwardKinematic(l1, l2, l3, theta1, theta2, theta3);
            Debug.Log($"End Effector Position: {endEffectorPosition}");

            theta1 = 270.0f * Deg2Rad;
            theta2 = 0.0f * Deg2Rad;
            theta3 = 0.0f * Deg2Rad;

            endEffectorPosition = __M_CalculateForwardKinematic(l1, l2, l3, theta1, theta2, theta3);
            Debug.Log($"End Effector Position: {endEffectorPosition}");

            m_endEffector.position = endEffectorPosition;
        }

        public static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
        {
            float t = InverseLerp(oldLow, oldHigh, input);
            return Lerp(newLow, newHigh, t);
        }

        private void Update()
        {
            float l1 = m_length1;
            float l2 = m_length2;
            float l3 = m_length3;

            var angle = m_body.transform.localRotation.eulerAngles.x;
            angle = Repeat(angle, 360);
            angle = Remap(angle, 0, 360, -180, 180);
            float theta1 = m_body.transform.localRotation.eulerAngles.x * Mathf.Deg2Rad;

            float theta2 = m_jointBody1.transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
            float theta3 = m_jointBody2.transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;

            Vector3 endEffectorPosition = __M_CalculateForwardKinematic(l1, l2, l3, theta1, theta2, theta3);
            m_endEffector.position = endEffectorPosition;
        }

        [System.Diagnostics.Contracts.Pure]
        private Vector3 __M_CalculateForwardKinematic(
            float l1, float l2, float l3,
            float theta1, float theta2, float theta3)
        {
            float x = l2 * Cos(theta2) + l3 * Cos(theta2 + theta3);
            float y = -l1 * Sin(theta1) + l2 * Sin(theta2) * Cos(theta1) +
                      l3 * Sin(theta2 + theta3) * Cos(theta1);
            float z = l1 * Cos(theta1) + l2 * Sin(theta2) * Sin(theta1) +
                      l3 * Sin(theta2 + theta3) * Sin(theta1);

            var localPosition = new Vector3(x, y, z);

            return localPosition;
        }
    }
}