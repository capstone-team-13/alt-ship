using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelemetryConnectionStatusUI : MonoBehaviour
{
    [Header("Connect")] [SerializeField]
    private string m_connectedMessage = "Connected to Server: Data is being collected.";

    [SerializeField] private Sprite m_connectedSprite;

    [Header("Disconnect")] [SerializeField]
    private string m_disconnectedMessage = "Not Connected: No data is being sent.";

    [SerializeField] private Sprite m_disconnectedSprite;

    [Header("Refs.")] [SerializeField] private TMP_Text m_messageText;
    [SerializeField] private Image m_connectionImage;

    public void Connect(int sessionId)
    {
        bool hasNetworkConnection = sessionId >= 0;
        m_messageText.text = hasNetworkConnection ? m_connectedMessage : m_disconnectedMessage;
        m_connectionImage.sprite = hasNetworkConnection ? m_connectedSprite : m_disconnectedSprite;
    }

    public void Disconnect(string message)
    {
        m_messageText.text = m_disconnectedMessage;
        m_connectionImage.sprite = m_disconnectedSprite;
    }
}