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
    public void Active(IInteractable interactable)
    {
        m_group.alpha = 1;
        m_keycode.text = interactable.KeyName;
        m_actionName.text = interactable.InteractionName;
    }

    [UsedImplicitly]
    public void Deactivate(IInteractable interactable)
    {
        m_group.alpha = 0;
    }
}