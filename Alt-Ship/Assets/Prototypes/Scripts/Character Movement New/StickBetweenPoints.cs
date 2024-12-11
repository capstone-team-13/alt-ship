using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class StickBetweenPoints : MonoBehaviour
    {
        public Transform m_start;
        public Transform m_end;
        public Transform m_mesh;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            m_mesh.position = m_start.position;
            var direction = (m_start.position - m_end.position).normalized;
            if (Vector3.Dot(direction, Vector3.up) < 0)
            {
                direction = -direction;
            }

            Quaternion currentRotation = m_mesh.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            m_mesh.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 10f);

        }
    }
}