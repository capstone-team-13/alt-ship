using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace EE.Prototype.CC
{
    public class FabrikSolverTest : MonoBehaviour
    {
        [SerializeField] private Transform[] m_joints;
        [SerializeField] private Transform m_target;

        private float3[] m_positions;
        private float[] m_lengths;

        [UsedImplicitly]
        private void Start()
        {
            m_positions = new float3[m_joints.Length];
            m_lengths = new float[m_joints.Length - 1];

            for (int i = 0; i < m_joints.Length - 1; i++)
            {
                Vector3 currentJoint = m_joints[i].position;
                Vector3 nextJoint = m_joints[i + 1].position;

                m_positions[i] = currentJoint;
                m_lengths[i] = Vector3.Magnitude(nextJoint - currentJoint);
            }

            m_positions[^1] = m_joints[^1].position;
        }

        [UsedImplicitly]
        private void Update()
        {
            FabrikSolver solver = new FabrikSolver.Builder().SetJoints(m_positions).SetLengths(m_lengths)
                .SetTarget(m_target.position).Build();

            solver.Solve();

            for (int i = 0; i < m_joints.Length; i++)
                m_joints[i].position = solver.Result[i];
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            foreach (Transform joint in m_joints)
                Gizmos.DrawWireSphere(joint.position, 0.25f);

            for (int i = 0; i < m_joints.Length - 1; i++)
                Gizmos.DrawLine(m_joints[i].position, m_joints[i + 1].position);
        }
    }
}