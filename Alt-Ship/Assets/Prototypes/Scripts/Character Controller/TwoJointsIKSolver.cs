using UnityEngine;

namespace EE.Prototype.CC
{
    public class TwoJointsIKSolver : MonoBehaviour
    {
        [SerializeField] private Transform m_joint1;
        [SerializeField] private Transform m_joint2;
        [SerializeField] private float m_l1;
        [SerializeField] private float m_l2;

        // Output
        private float m_theta1;
        private float m_theta2;

        private void Solve()
        {
        }
    }
}