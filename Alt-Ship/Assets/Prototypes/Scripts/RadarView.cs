using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EE.Prototype.OOP
{
    public class RadarView : MonoBehaviour
    {
        [Header("Model")]
        [SerializeField] private RadarModule m_radar;

        [Header("Refs")]
        [SerializeField] private RectTransform m_backgroundTransform;

        // TODO: Detection Generation
        // [SerializeField] private Sprite m_detectionPrefab;
        // [SerializeField] private RectTransform m_detectionPlaceHolder;
        [SerializeField] private List<RectTransform> m_detections;
        [SerializeField] private List<Image> m_detectionSprites;

        private void LateUpdate()
        {
            Render();
        }

        // TODO: Refactor for efficiency
        private void Render()
        {
            for (int i = 0; i < m_radar.HitCount; ++i)
            {
                var collider = m_radar.Results[i];
                var position = collider.transform.position;

                var relativePosition = position - m_radar.transform.position;

                var mapPosition = new Vector2(
                    relativePosition.x * (m_backgroundTransform.rect.width / 2.0f) / m_radar.Radius, 
                    relativePosition.z * (m_backgroundTransform.rect.height / 2.0f) / m_radar.Radius
                );

                m_detections[i].localPosition = mapPosition;

                Color color = m_detectionSprites[i].color;
                color.a = 1.0f;
                m_detectionSprites[i].color = color;
            }

            for (int i = m_radar.HitCount; i < 3; ++i)
            {
                Color color = m_detectionSprites[i].color;
                color.a = 0.0f;
                m_detectionSprites[i].color = color;
            }
        }
    }
}
