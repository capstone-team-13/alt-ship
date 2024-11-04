using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.CI
{
    public class CannonInteraction : MonoBehaviour
    {
        [SerializeField] private MovementSolver m_player;
        [SerializeField] private Vector3 m_origin;

        private bool m_grabbing;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            Vector3 toPlayer = transform.position - m_player.Base.position;
            var distanceToPlayer = toPlayer.magnitude;

            switch (distanceToPlayer)
            {
                case > 8.0f when m_grabbing:
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 5);
                    if (Quaternion.Angle(transform.rotation, Quaternion.identity) < 1.0f) // 当旋转非常接近初始状态时
                    {
                        transform.rotation = Quaternion.identity;
                        m_grabbing = false;
                    }

                    break;
                case < 5.0f:
                    Vector3 directionToPlayer = m_player.Base.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    float yRotation = targetRotation.eulerAngles.y;
                    Quaternion finalRotation = Quaternion.Euler(0, yRotation, 0);

                    transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 5);

                    m_grabbing = true;
                    break;
            }
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward);
        }
    }
}