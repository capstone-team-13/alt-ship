using System.Linq;
using Unity.Mathematics;

namespace EE.Prototype.PBA
{
    public class FabrikSolver
    {
        // Input
        private float3[] m_joints;
        private float[] m_lengths;

        private float3 m_root;
        private float3 m_target;

        private float m_torrence = 1e-1f;
        private int m_maxIteration = 1000;

        // Output
        public float3[] Result => m_joints;

        public void Solve()
        {
            float targetLength = math.length(m_target - m_joints[0]);
            float chainLength = m_lengths.Sum();

            bool reachable = targetLength < chainLength;

            if (reachable)
            {
                bool solved = false;
                float currentDistance = math.INFINITY;

                for (int i = 0; i < m_maxIteration; i++)
                {
                    __M_ForwardReaching();
                    __M_BackwardReaching();

                    currentDistance = math.length(m_target - m_joints[^1]);

                    if (currentDistance <= m_torrence)
                    {
                        solved = true;
                        break;
                    }
                }

                if (!solved)
                {
                    throw new System.Exception(
                        $"Unable to solve the IK problem within {m_maxIteration} iterations. " +
                        $"Final distance from target: {currentDistance} units, " +
                        $"which exceeds the tolerance limit of {m_torrence} units.");
                }
            }
            else
            {
                for (int i = 0; i < m_joints.Length - 1; i++)
                {
                    float3 direction = math.normalize(m_target - m_joints[i]);
                    float3 position = m_joints[i] + direction * m_lengths[i];
                    m_joints[i + 1] = position;
                }
            }
        }

        private FabrikSolver()
        {
        }

        #region Internal

        private void __M_ForwardReaching()
        {
            m_joints[^1] = m_target;

            for (int i = m_joints.Length - 1; i > 0; i--)
            {
                float3 current = m_joints[i];
                float3 previous = m_joints[i - 1];
                float currentDistance = math.length(current - previous);
                float lambda = m_lengths[i - 1] / currentDistance;

                m_joints[i - 1] = current + lambda * (previous - current);
            }
        }

        private void __M_BackwardReaching()
        {
            m_joints[0] = m_root;

            for (int i = 0; i < m_joints.Length - 1; i++)
            {
                float3 current = m_joints[i];
                float3 next = m_joints[i + 1];
                float currentDistance = math.length(next - current);
                float lambda = m_lengths[i] / currentDistance;

                m_joints[i + 1] = current + lambda * (next - current);
            }
        }

        #endregion

        #region

        public class Builder
        {
            private FabrikSolver m_solver = new();

            public Builder SetJoints(float3[] joints)
            {
                m_solver.m_joints = joints;
                return this;
            }

            public Builder SetLengths(float[] lengths)
            {
                m_solver.m_lengths = lengths;
                return this;
            }

            public Builder SetTarget(float3 target)
            {
                m_solver.m_target = target;
                return this;
            }

            public Builder SetTorrence(float torrence)
            {
                m_solver.m_torrence = torrence;
                return this;
            }

            public Builder SetMaxIteration(int maxIteration)
            {
                m_solver.m_maxIteration = maxIteration;
                return this;
            }

            public FabrikSolver Build()
            {
                if (m_solver.m_joints == null || m_solver.m_joints.Length < 1)
                    throw new System.ArgumentException("Joints array must contain at least one element.");
                if (m_solver.m_lengths == null)
                    throw new System.ArgumentException("Lengths array cannot be null.");
                if (m_solver.m_lengths.Length != m_solver.m_joints.Length - 1)
                    throw new System.ArgumentException("Lengths array must be one less than joints array.");

                m_solver.m_root = m_solver.m_joints[0];

                return m_solver;
            }
        }

        #endregion
    }
}