using EE.Interactions;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class InteractionKeyUI : MonoBehaviour
{
    [Header("Reference")] [SerializeField] private CanvasGroup m_group;
    [SerializeField] private TMP_Text m_keycode;
    [SerializeField] private TMP_Text m_actionName;

    [UsedImplicitly]
    public void Active(Interactable interactable)
    {
        m_group.alpha = 1;
        m_keycode.text = interactable.KeyCode.ToString();
        m_actionName.text = interactable.InteractionName;
    }

    [UsedImplicitly]
    public void Deactivate(Interactable interactable)
    {
        m_group.alpha = 0;
    }
}