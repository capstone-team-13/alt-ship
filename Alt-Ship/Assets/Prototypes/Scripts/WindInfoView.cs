using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EE.Prototype.OOP
{
    public class WindInfoView : MonoBehaviour
    {
        [Header("Model")]
        [SerializeField] ShipModel m_model;
        [SerializeField] WindModule m_wind;

        [Header("UI Components")]
        [SerializeField] Image m_windDirectionImage;
        [SerializeField] TMP_Text m_windSpeed;
        [SerializeField] TMP_Text m_apparentWindAngle;

        [SerializeField] float rotationSpeed = 5f;

        private void LateUpdate()
        {
            RotateWindDirectionIndicator();

            m_windSpeed.text = $"{m_wind.Speed:F1} m/s";

            m_apparentWindAngle.text = $"{m_model.ApparentWindAngle:F1}°";
        }

        private void RotateWindDirectionIndicator()
        {
            var targetRotationZ = CalculateWindDirectionInAngle(m_wind.Direction);
            var targetRotation = Quaternion.Euler(0, 0, targetRotationZ);

            var currentRotation = m_windDirectionImage.rectTransform.rotation;
            var smoothRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);

            m_windDirectionImage.rectTransform.rotation = smoothRotation;
        }

        // Return rotation Z of the wind direction
        private float CalculateWindDirectionInAngle(Vector3 windDirection)
        {
            float angleInRadians = Mathf.Atan2(windDirection.z, windDirection.x);
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
            if (angleInDegrees < 0) angleInDegrees += 360;

            return angleInDegrees;
        }
    }
}
