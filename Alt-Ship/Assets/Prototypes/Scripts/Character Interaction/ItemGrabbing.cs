using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.CI
{
    public class ItemGrabbing : MonoBehaviour
    {
        [Header("Configs")] [SerializeField] private float m_springConstant;
        [SerializeField] private float m_dampingCoefficient;
        [SerializeField] private Vector3 m_targetPosition;

        [Header("Refs.")] [SerializeField] private Rigidbody m_rigidbody = null;
        [SerializeField] private MovementSolver m_player;

        private bool m_grabbing;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 toPlayer = m_targetPosition - m_player.Base.position;
            var distanceToPlayer = toPlayer.magnitude;

            Vector3 targetPosition;
            switch (distanceToPlayer)
            {
                case > 10.0f when m_grabbing:
                    targetPosition = m_targetPosition;
                    m_grabbing = false;
                    break;
                case < 4.0f:
                    targetPosition = m_player.GrabPivot.position;
                    m_grabbing = true;
                    break;
                default:
                    targetPosition = m_targetPosition;
                    break;
            }

            Vector3 springForce = (targetPosition - m_rigidbody.position) * m_springConstant;
            Vector3 convergenceForce = (Vector3.zero - m_rigidbody.velocity) * m_dampingCoefficient;

            Vector3 force = springForce + convergenceForce;
            m_rigidbody.AddForce(force, ForceMode.Force);
        }
    }
}