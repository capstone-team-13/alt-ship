using JetBrains.Annotations;
using System;
using UnityEngine;

namespace EE.Prototype.CM
{
    public class TrajectoryOnly : MonoBehaviour
    {
        #region Editor API

        [Header("Configs")] [SerializeField] private LayerMask m_rayCastLayerMask;

        [Header("Refs.")] [SerializeField] private Transform m_base;
        [SerializeField] private Transform[] m_foots;

        #endregion

        #region API

        public Transform Base => m_base;

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void Awake()
        {
            var length = m_foots.Length;
            m_targetPositions = new Vector3[length];

            for (var i = 0; i < m_foots.Length; i++)
                m_targetPositions[i] = m_foots[i].position;
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
            __M_Move();
        }

        [UsedImplicitly]
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.green;

            for (var i = 0; i < m_foots.Length; i++)
            {
                // var rayOrigin = foot.position + Vector3.up * 1.0f;
                var rayOrigin = new Vector3(m_base.position.x + m_baseToFoot[i].x, m_base.position.y,
                    m_base.position.z);
                var rayDirection = Vector3.down;
                const float rayDistance = 10.0f;

                // Draw the raycast line for each foot
                Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * rayDistance);

                // Check if the ray hits and visualize
                if (!Physics.Raycast(rayOrigin, rayDirection, out var hit, rayDistance, m_rayCastLayerMask)) continue;

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rayOrigin, 0.1f);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, 0.1f);
            }
        }

        #endregion

        #region Internal

        private Vector3[] m_targetPositions;

        private Vector3[] m_baseToFoot;

        private int m_activeFootIndex;
        private int m_resetFootIndex;

        private void __M_Move()
        {
            __M_UpdateFootTargets();

            __M_SmoothlyMoveFeetToTargets();

            __M_UpdateActiveFootIndexIfNeeded();
        }

        private void __M_UpdateFootTargets()
        {
            const float rayDistance = 10.0f;

            for (var i = 0; i < m_foots.Length; i++)
            {
                var rayOrigin = new Vector3(m_base.position.x + m_baseToFoot[i].x, m_base.position.y,
                    m_base.position.z);
                var rayDirection = Vector3.down;

                if (Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, rayDistance, m_rayCastLayerMask))
                {
                    var hitPoint = hitInfo.point;
                    var regularPoint = m_base.position + m_baseToFoot[i];

                    // Choose the closer position between the hit point and the regular point
                    m_targetPositions[i] =
                        Vector3.Distance(m_base.position, hitPoint) < Vector3.Distance(m_base.position, regularPoint)
                            ? hitPoint
                            : regularPoint;

                    // Align foot rotation with the hit normal
                    // foot.rotation = Quaternion.LookRotation(foot.forward, hitInfo.normal);
                }
                else
                {
                    var basePosition = m_base.position + m_baseToFoot[i];
                    // Default to base position if no hit is detected
                    m_targetPositions[i] = basePosition;
                }
            }
        }

        private void __M_SmoothlyMoveFeetToTargets()
        {
            // for (var i = 0; i < m_foots.Length; i++)
            //     m_foots[i].position = Vector3.Lerp(m_foots[i].position, m_targetPositions[i], Time.deltaTime * 5f);
            m_foots[m_activeFootIndex].position = Vector3.Lerp(m_foots[m_activeFootIndex].position,
                m_targetPositions[m_activeFootIndex], Time.deltaTime * 5f);
        }

        private void __M_UpdateActiveFootIndexIfNeeded()
        {
            var theOtherFootIndex = (m_activeFootIndex + 1) % m_foots.Length;
            var activeFoot = m_foots[m_activeFootIndex];

            // Check if it's time to switch the active foot
            if (__M_IsActiveFootReachedTarget(activeFoot) || __M_IsOtherFootTooFar(theOtherFootIndex) ||
                __M_AreFeetTooClose(m_activeFootIndex, theOtherFootIndex))
                m_activeFootIndex = (m_activeFootIndex + 1) % m_foots.Length;
        }

        private bool __M_IsActiveFootReachedTarget(Transform activeFoot)
        {
            return (activeFoot.position - m_targetPositions[m_activeFootIndex]).magnitude < 0.1f;
        }

        private bool __M_IsOtherFootTooFar(int otherFootIndex)
        {
            return (m_foots[otherFootIndex].position - m_base.position).magnitude >= 3.0f;
        }

        private bool __M_AreFeetTooClose(int footIndex, int otherFootIndex)
        {
            return (m_foots[footIndex].position - m_foots[otherFootIndex].position).magnitude <= 0.75f;
        }

        [System.Diagnostics.Contracts.Pure]
        private static Vector3 __M_GetUserInput()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var movement = new Vector3(horizontal, 0.0f, vertical);

            return movement;
        }

        #endregion
    }
}