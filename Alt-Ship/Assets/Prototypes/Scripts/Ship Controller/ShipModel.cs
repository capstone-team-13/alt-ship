using UnityEngine;

namespace EE.Prototype.OOP
{
    public class ShipModel : MonoBehaviour
    {
        [Header("Refs.")]
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private WindModule m_windModule;

        [SerializeField] private float m_speed;
        [SerializeField] private Vector3 m_seilDirection;

        private float m_windEffect;
        public float WindEffect => m_windEffect;

        private float m_apparentWindAngle;
        public float ApparentWindAngle => m_apparentWindAngle;

        #region Unity Callbacks

        // TODO: Refactor into Ship Controller
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float lerpSpeed = 5f;
        private Vector3 targetEulerAngles;
        private void Start()
        {
            targetEulerAngles = transform.localEulerAngles;
        }
        private void Update()
        {
            Vector3 currentEulerAngles = transform.localEulerAngles;

            if (currentEulerAngles.y > 180)
            {
                currentEulerAngles.y -= 360;
            }

            float rotationDelta = rotationSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.A))
            {
                targetEulerAngles.y = Mathf.Max(targetEulerAngles.y - rotationDelta, -60);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                targetEulerAngles.y = Mathf.Min(targetEulerAngles.y + rotationDelta, 60);
            }

            Vector3 smoothEulerAngles = Vector3.Lerp(currentEulerAngles, targetEulerAngles, lerpSpeed * Time.deltaTime);
            transform.localEulerAngles = smoothEulerAngles;

            m_seilDirection = transform.forward;
        }

        private void FixedUpdate()
        {
            m_apparentWindAngle = CalculateApparentWindAngle();

            var windVelocity = m_windModule.Velocity;
            var selfVelocity = m_speed * m_seilDirection;
            m_rigidbody.velocity = selfVelocity + windVelocity;
        }

        #endregion

        #region Main Methods

        public Vector3 CalcuateWindVelocity()
        {
            float angle = Vector3.Angle(m_seilDirection, m_windModule.Direction);

            // Calculate wind effect based on how perpendicular the sail is to the wind
            m_windEffect = Mathf.Sin(angle * Mathf.Deg2Rad);

            // Use dot product to determine the direction of the wind (positive or negative)
            // If dot product is negative, the wind is coming from behind (angle close to 180 degrees)
            float windDirection = Mathf.Sign(Vector3.Dot(m_seilDirection, m_windModule.Direction));

            // Apply wind effect and reverse direction if necessary
            return m_windModule.Velocity * m_windEffect * windDirection;
        }

        private float CalculateApparentWindAngle()
        {
            float apparentWindAngle = Vector3.SignedAngle(transform.forward, m_windModule.Direction, Vector3.up);
            return apparentWindAngle;
        }

        #endregion
    }
}
