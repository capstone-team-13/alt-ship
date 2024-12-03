using JetBrains.Annotations;
using System;
using UnityEngine;

namespace EE.Prototype.CM
{
    public class TrajectoryOnly : MonoBehaviour
    {
        [Header("Configs")] [SerializeField] private float stepTime = 0.25f;
        [SerializeField] private float m_liftHeight = 0.3f;
        [SerializeField] private float m_stepLength = 0.5f;
        [SerializeField] private float m_legLiftHeight = 0.5f;
        [SerializeField] private float m_swingPhasePeriod = 2f;
        [SerializeField] private LayerMask m_rayCastLayer;

        [Header("Refs.")] [SerializeField] private Transform m_base;
        [SerializeField] private Transform m_grabPivot;
        [SerializeField] private Transform[] m_foots;

        private Vector3 m_startFrom;

        private Vector3[] m_startPositions;

        private Vector3[] m_targetPositions;

        private Vector3[] m_baseToFoot;

        private int m_activeFootIndex;
        private bool m_isMoving;
        private float m_timeSinceLastStep;
        private int m_resetFootIndex;

        public Transform Base => m_base;
        public Transform GrabPivot => m_grabPivot;

        [UsedImplicitly]
        private void Awake()
        {
            var length = m_foots.Length;
            m_startPositions = new Vector3[length];
            m_targetPositions = new Vector3[length];

            for (var i = 0; i < m_foots.Length; i++)
                m_startPositions[i] = m_targetPositions[i] = m_foots[i].position;
        }

        [UsedImplicitly]
        private void Start()
        {
            m_baseToFoot = new Vector3[m_foots.Length];

            for (var i = 0; i < m_foots.Length; i++)
                m_baseToFoot[i] = m_foots[i].position - m_base.position;
        }

        [UsedImplicitly]
        private void Update()
        {
            var input = __M_GetUserInput();

            m_base.Translate(input * (2.0f * Time.deltaTime));

            if (input.magnitude != 0.0f)
            {
                m_isMoving = true;
                __M_Move();
            }
            else if (m_isMoving) Reset();

            // __M_Move();
        }


        private void __M_Move()
        {
            // var gait = __M_GenerateGaitPath(0);
            // m_base.transform.position = m_startFrom + gait;
            //
            // if (!(Time.time % m_swingPhasePeriod + 0.002 >= m_swingPhasePeriod)) return;
            // m_startFrom = m_base.transform.position;
            // Debug.Log("T");

            UpdateFootTargets();

            SmoothlyMoveFeetToTargets();

            UpdateActiveFootIndexIfNeeded();
        }

        private void UpdateFootTargets()
        {
            for (var i = 0; i < m_foots.Length; i++)
            {
                var foot = m_foots[i];
                var rayOrigin = foot.position + Vector3.up * 1.0f;
                var rayDirection = Vector3.down;
                var rayDistance = 1 + m_legLiftHeight;

                if (Physics.Raycast(rayOrigin, rayDirection, out var hit, rayDistance, m_rayCastLayer))
                {
                    var hitPoint = hit.point;
                    var regularPoint = m_base.position + m_baseToFoot[i];

                    m_targetPositions[i] =
                        Vector3.Distance(m_base.position, hitPoint) < Vector3.Distance(m_base.position, regularPoint)
                            ? hitPoint
                            : regularPoint;

                    foot.rotation = Quaternion.LookRotation(foot.forward, hit.normal);
                }
                else m_targetPositions[i] = m_base.position + m_baseToFoot[i];
            }
        }

        private void SmoothlyMoveFeetToTargets()
        {
            // for (var i = 0; i < m_foots.Length; i++)
            //     m_foots[i].position = Vector3.Lerp(m_foots[i].position, m_targetPositions[i], Time.deltaTime * 5f);
            m_foots[m_activeFootIndex].position = Vector3.Lerp(m_foots[m_activeFootIndex].position,
                m_targetPositions[m_activeFootIndex], Time.deltaTime * 5f);
        }

        private void UpdateActiveFootIndexIfNeeded()
        {
            var theOtherFootIndex = (m_activeFootIndex + 1) % m_foots.Length;
            var activeFoot = m_foots[m_activeFootIndex];

            // Check if it's time to switch the active foot
            if (IsActiveFootReachedTarget(activeFoot) || IsOtherFootTooFar(theOtherFootIndex))
                m_activeFootIndex = (m_activeFootIndex + 1) % m_foots.Length;
        }

        private bool IsActiveFootReachedTarget(Transform activeFoot)
        {
            return (activeFoot.position - m_targetPositions[m_activeFootIndex]).magnitude < 0.1f;
        }

        private bool IsOtherFootTooFar(int otherFootIndex)
        {
            return (m_foots[otherFootIndex].position - m_base.position).magnitude >= 3.0f;
        }

        private void Reset()
        {
            var foot = m_foots[m_resetFootIndex];
            var basePosition = m_base.position + m_baseToFoot[m_resetFootIndex];
            var rayOrigin = m_base.position;
            var rayDirection = (basePosition - rayOrigin).normalized;
            var rayDistance = Vector3.Distance(rayOrigin, basePosition);

            Vector3 targetPosition;

            if (Physics.Raycast(rayOrigin, rayDirection, out var hit, rayDistance, m_rayCastLayer))
            {
                targetPosition = hit.point;
            }
            else
            {
                targetPosition = basePosition;
            }

            foot.position = Vector3.Lerp(foot.position, targetPosition, Time.deltaTime * 5.0f);

            if (!((foot.position - targetPosition).magnitude < 0.1f)) return;

            m_resetFootIndex = (m_resetFootIndex + 1) % m_foots.Length;

            if (m_resetFootIndex == 0) m_isMoving = false;
        }


        [System.Diagnostics.Contracts.Pure]
        private Vector3 __M_GetUserInput()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var movement = new Vector3(horizontal, 0.0f, vertical);

            return movement;
        }

        private Vector3 __M_GenerateGaitPath(float timeSinceLastStep)
        {
            timeSinceLastStep = Time.time % m_swingPhasePeriod;
            var x = m_stepLength * (timeSinceLastStep / m_swingPhasePeriod -
                                    0.5f * Mathf.Sin(2.0f * Mathf.PI * timeSinceLastStep / m_swingPhasePeriod));
            var y = m_legLiftHeight *
                    (0.5f - 0.5f * Mathf.Cos(2.0f * Mathf.PI * timeSinceLastStep / m_swingPhasePeriod));
            return new Vector3(x, y, 0);
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            foreach (var foot in m_foots)
            {
                var rayOrigin = foot.position + Vector3.up * 1.0f;
                var rayDirection = Vector3.down;
                var rayDistance = 1 + m_legLiftHeight;

                // Draw the raycast line for each foot
                Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * rayDistance);

                // Check if the ray hits and visualize
                if (!Physics.Raycast(rayOrigin, rayDirection, out var hit, rayDistance, m_rayCastLayer)) continue;

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rayOrigin, 0.1f);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, 0.1f);
            }
        }
    }
}