using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.CM
{
    public class BodyAdjuster : MonoBehaviour
    {
        private Vector3 lastPosition;
        public float maxTiltAngle = 15f;
        public float tiltSpeed = 10f;

        [UsedImplicitly]
        private void Start()
        {
            lastPosition = transform.position;
        }

        [UsedImplicitly]
        private void Update()
        {
            var velocity = (transform.position - lastPosition) / Time.deltaTime;

            if (velocity.sqrMagnitude > 0.0001f)
            {
                var velocityDirection = velocity.normalized;

                var angle = Vector3.Angle(velocityDirection, transform.forward);
                if (angle is <= 45f or >= 135f)
                {
                    var dotProduct = Vector3.Dot(velocityDirection, transform.forward);
                    var tiltDirection = dotProduct >= 0 ? -1 : 1;

                    var tiltAngle = Mathf.Lerp(0, maxTiltAngle, Mathf.Clamp01(velocity.sqrMagnitude / 4.0f));

                    var targetTilt = Quaternion.Euler(-tiltDirection * tiltAngle, 0, 0);

                    transform.localRotation =
                        Quaternion.Slerp(transform.localRotation, targetTilt, tiltSpeed * Time.deltaTime);
                }
                else
                {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity,
                        tiltSpeed * Time.deltaTime);
                }
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity,
                    tiltSpeed * Time.deltaTime);
            }


            lastPosition = transform.position;
        }
    }
}