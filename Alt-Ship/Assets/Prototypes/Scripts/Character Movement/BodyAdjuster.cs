using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.CM
{
    public class BodyAdjuster : MonoBehaviour
    {
        private Vector3 m_lastPosition;
        public float maxTiltAngle = 15f;
        public float tiltSpeed = 10f;

        [SerializeField] private Transform m_body;
        [SerializeField] private LayerMask m_rayCastLayer;

        [UsedImplicitly]
        private void Start()
        {
            m_lastPosition = transform.position;
        }

        [UsedImplicitly]
        private void Update()
        {
            // __M_Tilt();
            __M_HeightConstraint();
        }

        // private void __M_Tilt()
        // {
        //     var velocity = (transform.position - m_lastPosition) / Time.deltaTime;
        //
        //     if (velocity.sqrMagnitude > 0.0001f)
        //     {
        //         var velocityDirection = velocity.normalized;
        //
        //         var angle = Vector3.Angle(velocityDirection, transform.forward);
        //         if (angle is <= 45f or >= 135f)
        //         {
        //             var dotProduct = Vector3.Dot(velocityDirection, transform.forward);
        //             var tiltDirection = dotProduct >= 0 ? -1 : 1;
        //
        //             var tiltAngle = Mathf.Lerp(0, maxTiltAngle, Mathf.Clamp01(velocity.sqrMagnitude / 4.0f));
        //
        //             var targetTilt = Quaternion.Euler(-tiltDirection * tiltAngle, 0, 0);
        //
        //             transform.localRotation =
        //                 Quaternion.Slerp(transform.localRotation, targetTilt, tiltSpeed * Time.deltaTime);
        //         }
        //         else
        //         {
        //             transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity,
        //                 tiltSpeed * Time.deltaTime);
        //         }
        //     }
        //     else
        //     {
        //         transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity,
        //             tiltSpeed * Time.deltaTime);
        //     }
        // }

        private void __M_HeightConstraint()
        {
            Vector3 rayOrigin = m_body.position;
            Vector3 rayDirection = Vector3.down;
            const float rayDistance = 10.0f;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hitInfo, rayDistance, m_rayCastLayer))
            {
                const float targetHeight = 2.25f;
                Vector3 targetPosition = hitInfo.point;
                targetPosition.y += targetHeight;

                m_body.position = Vector3.Lerp(m_body.position, targetPosition, Time.deltaTime * 5.0f);
            }
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            Vector3 rayOrigin = m_body.position;
            Vector3 rayDirection = Vector3.down;
            const float rayDistance = 10.0f;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(rayOrigin, 0.1f);

            if (Physics.Raycast(rayOrigin, rayDirection, out var hitInfo, rayDistance, m_rayCastLayer))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(rayOrigin, hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, 0.1f);
            }
            else
            {
                Debug.Log("Hit Nothing.");
            }
        }
    }
}