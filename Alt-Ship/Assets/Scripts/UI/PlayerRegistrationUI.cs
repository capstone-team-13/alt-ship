using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRegistrationUI : MonoBehaviour
{
    [Header("Configs")] [SerializeField] private Color m_joinedColor;
    [SerializeField] private Color m_exitedColor;
    [SerializeField] private int m_index = -1;

    [Header("Refs.")] [SerializeField] private KeyHintUI m_joinKeyHint;
    [SerializeField] private KeyHintUI m_exitKeyHint;

    [SerializeField] private ReadyIndicatorUI m_readyIndicator;

    [SerializeField] private Animator m_playerAnimator;
    private List<string> m_joinedAnimationTriggers = new();
    private List<string> m_exitedAnimationTriggers = new();
    private void Start()
    {
        if (m_playerAnimator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        var parameters = m_playerAnimator.parameters;

        const string joinedAnimationPrefix = "$(Joined)";
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            bool isTrigger = parameter.type == AnimatorControllerParameterType.Trigger;
            if (!isTrigger || !parameter.name.StartsWith(joinedAnimationPrefix)) continue;

            m_joinedAnimationTriggers.Add(parameter.name);
        }

        const string exitedAnimationPrefix = "$(Exited)";
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            bool isTrigger = parameter.type == AnimatorControllerParameterType.Trigger;
            if (!isTrigger || !parameter.name.StartsWith(exitedAnimationPrefix)) continue;

            m_exitedAnimationTriggers.Add(parameter.name);
        }

        m_playerAnimator.SetBool("Mirror", m_index != 0);
    }


    [UsedImplicitly]
    private void OnEnable()
    {
        LevelManager.PlayerEventBus.SubscribeTo<PlayerJoinedEvent>(OnPlayerJoined);
        LevelManager.PlayerEventBus.SubscribeTo<PlayerExitedEvent>(OnPlayerExited);
    }

    [UsedImplicitly]
    private void OnDisable()
    {
        LevelManager.PlayerEventBus.UnsubscribeFrom<PlayerJoinedEvent>(OnPlayerJoined);
        LevelManager.PlayerEventBus.UnsubscribeFrom<PlayerExitedEvent>(OnPlayerExited);
    }

    private void OnPlayerJoined(ref PlayerJoinedEvent eventData, GameObject target, GameObject source)
    {
        InputDevice device = eventData.Device;
        if (eventData.Index != m_index) return;

        if (device.displayName.Contains("Xbox"))
        {
            m_joinKeyHint.SetSpriteType(KeyHintUI.SpriteType.XBOX);
            m_exitKeyHint.SetSpriteType(KeyHintUI.SpriteType.XBOX);
        }
        else
        {
            m_joinKeyHint.SetSpriteType(KeyHintUI.SpriteType.PLAYSTATION);
            m_exitKeyHint.SetSpriteType(KeyHintUI.SpriteType.PLAYSTATION);
        }

        m_joinKeyHint.PlayPressButtonEffect();

        m_readyIndicator.SetColor(m_joinedColor, 1.0f);
        m_readyIndicator.SetReadyImageActive(true);

        int randomIndex = Random.Range(0, m_joinedAnimationTriggers.Count);
        m_playerAnimator.SetTrigger(m_joinedAnimationTriggers[randomIndex]);
    }

    private void OnPlayerExited(ref PlayerExitedEvent eventData, GameObject target, GameObject source)
    {
        if (eventData.Index != m_index) return;

        m_joinKeyHint.SetSpriteType(KeyHintUI.SpriteType.UNKNOWN);
        m_exitKeyHint.SetSpriteType(KeyHintUI.SpriteType.UNKNOWN);

        m_exitKeyHint.PlayPressButtonEffect();

        m_readyIndicator.SetColor(m_exitedColor, 1.0f);
        m_readyIndicator.SetReadyImageActive(false);
        
        int randomIndex = Random.Range(0, m_exitedAnimationTriggers.Count);
        m_playerAnimator.SetTrigger(m_exitedAnimationTriggers[randomIndex]);
    }
}