using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.CI
{
    public class MovementSolver : MonoBehaviour
    {
        [SerializeField] private Transform m_base;
        [SerializeField] private Transform m_leftHand;
        [SerializeField] private Transform m_rightHand;
        [SerializeField] private Transform[] m_foots;

        private float m_baseFootDistance = 2.25f;
        private Vector3[] m_baseToFoot;
        private int m_swingFoot = 0;

        [UsedImplicitly]
        private void Start()
        {
            m_baseToFoot = new Vector3[m_foots.Length];

            for (int i = 0; i < m_foots.Length; i++)
                m_baseToFoot[i] = m_foots[i].position - m_base.position;
        }

        [UsedImplicitly]
        private void Update()
        {
            Vector3 input = __M_GetUserInput();
            m_base.Translate(input * 5.0f * Time.deltaTime);
            m_leftHand.Translate(input * 5.0f * Time.deltaTime);
            m_rightHand.Translate(input * 5.0f * Time.deltaTime);

            if ((m_base.position - m_foots[m_swingFoot].position).magnitude > m_baseFootDistance * 1.25f)
            {
                float checkDistance = 1.0f;
                bool isGrounded = Physics.Raycast(m_foots[m_swingFoot].position, Vector3.down, out RaycastHit _,
                    checkDistance);

                Debug.Log(isGrounded);

                if (isGrounded)
                {
                    m_foots[m_swingFoot].position = m_base.position + m_baseToFoot[m_swingFoot];
                    m_swingFoot++;
                    m_swingFoot %= m_foots.Length;

                    int nextSwingFoot = m_swingFoot;

                    Vector3 initialPosition = m_foots[nextSwingFoot].position;
                    initialPosition.y += 1.0f;
                    m_foots[nextSwingFoot].position = initialPosition;

                    m_swingFoot = nextSwingFoot;
                }
                else m_foots[m_swingFoot].Translate(0, -1 * Time.deltaTime, 0);
            }
        }

        [System.Diagnostics.Contracts.Pure]
        private Vector3 __M_GetUserInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var movement = new Vector3(horizontal, 0.0f, vertical);
            return movement;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_foots[m_swingFoot].position, Vector3.down);
        }
    }
}