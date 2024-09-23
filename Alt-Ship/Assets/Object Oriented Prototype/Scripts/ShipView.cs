using TMPro;
using UnityEngine;

namespace EE.Prototype.OOP
{
    public class ShipView : MonoBehaviour
    {
        [Header("Model")]
        [SerializeField] ShipModel m_model;

        [Header("UI Components")]
        [SerializeField] TMP_Text m_windEffectAmountText;

        private void Update()
        {
            m_windEffectAmountText.text = $"Wind Effect: {m_model.WindEffect * 100:F2}";
        }
    }
}
