using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PBA
{
    public class SingleLegSolver : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Rigidbody m_joint1;
        [SerializeField] private Rigidbody m_joint2;
        [SerializeField] private float m_jointLength1;
        [SerializeField] private float m_jointLength2;
        [SerializeField] private Transform m_endEffector;
        [SerializeField] private float m_springConstant;
        [SerializeField] private float m_dampingConstant;
        [SerializeField] private Vector3 m_targetHeight;

        [UsedImplicitly]
        private void Update()
        {
            __M_ForwardKinematic();
        }

        private void __M__CalculateVirtualForce()
        {
            Vector3 springForce = m_springConstant * (m_targetHeight - m_body.transform.position);
            Vector3 convergenceForce = m_dampingConstant * (Vector3.zero - m_body.velocity);

            Vector3 force = springForce + convergenceForce;
            m_body.AddForce(force, ForceMode.Force);
        }

        private void __M_ForwardKinematic()
        {
            float theta1 = m_joint1.transform.eulerAngles.z * Mathf.Deg2Rad;
            float theta2 = m_joint2.transform.eulerAngles.z * Mathf.Deg2Rad;

            var localPosition = new Vector3(
                m_jointLength1 * Mathf.Sin(theta1) + m_jointLength2 * Mathf.Sin(theta2),
                m_jointLength1 * Mathf.Cos(theta1) + m_jointLength2 * Mathf.Cos(theta2),
                0
            );
            localPosition.y *= -1;

            Vector3 worldPosition = localPosition + m_joint1.position;
            m_endEffector.position = worldPosition;
        }
    }
}