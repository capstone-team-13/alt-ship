using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.CI
{
    public class MovementSolver : MonoBehaviour
    {
        [SerializeField] private Transform m_base;
        [SerializeField] private Transform m_grabPivot;
        [SerializeField] private Transform[] m_foots;

        private Vector3[] m_baseToFoot;

        private int m_swingFoot = 0;
        private int activeFootIndex;
        private bool isMoving;
        [SerializeField] private float stepTime = 0.25f;
        private float timeSinceLastStep;
        private int resetFootIndex;

        public Transform Base => m_base;
        public Transform GrabPivot => m_grabPivot;

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

            if (input.magnitude != 0)
            {
                isMoving = true;
                MoveFeetBasedOnInput();
            }
            else if (isMoving) ResetFeetToBase();
        }

        private void MoveFeetBasedOnInput()
        {
            Transform activeFoot = m_foots[activeFootIndex];
            Vector3 targetPosition = m_base.position + m_baseToFoot[activeFootIndex];

            float sinOffset = Mathf.Sin(Time.time * Mathf.PI * 2.0f) * 0.25f;
            targetPosition.y += sinOffset;

            activeFoot.position = Vector3.Lerp(activeFoot.position, targetPosition, Time.deltaTime * 5f);

            timeSinceLastStep += Time.deltaTime;

            if (timeSinceLastStep >= stepTime || (activeFoot.position - targetPosition).magnitude < 0.1f)
            {
                activeFootIndex = (activeFootIndex + 1) % m_foots.Length;
                timeSinceLastStep = 0f;
            }
        }

        private void ResetFeetToBase()
        {
            Transform foot = m_foots[resetFootIndex];
            Vector3 basePosition = m_base.position + m_baseToFoot[resetFootIndex];
            foot.position = Vector3.Lerp(foot.position, basePosition, Time.deltaTime * 5.0f);

            if (!((foot.position - basePosition).magnitude < 0.1f)) return;

            resetFootIndex = (resetFootIndex + 1) % m_foots.Length;

            if (resetFootIndex == 0) isMoving = false;
        }


        [System.Diagnostics.Contracts.Pure]
        private Vector3 __M_GetUserInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var movement = new Vector3(horizontal, 0.0f, vertical);

            return movement;
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_foots[m_swingFoot].position, Vector3.down);
        }
    }
}