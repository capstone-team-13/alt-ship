using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private TMP_Text m_readyText;

    public void SetColor(Color color)
    {
        m_backgroundImage.color = color;
    }

    public void SetText(string text)
    {
        m_readyText.text = text;
    }
}