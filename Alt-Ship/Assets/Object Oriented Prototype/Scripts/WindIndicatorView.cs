using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EE.Prototype.OOP
{
    public class WindIndicatorView : MonoBehaviour
    {
        [Header("Model")]
        [SerializeField] ShipModel m_model;

        [Header("UI Components")]
        [SerializeField] Image m_windDirectionImage;
        [SerializeField] TMP_Text m_windEffectAmountText;
        [SerializeField] TMP_Text m_apparentWindAngle;

        private void Update()
        {
            m_windEffectAmountText.text = $"Wind Effect: {m_model.WindEffect * 100:F2}";
        }

        private void CalculateWindDirectionInAngle(Vector3 winDirection)
        {

        }

        private void CalculateApparentWindAngle()
        {

        }
    }
}
