using System.Collections;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerDeviceManager : SingletonBehaviour<PlayerDeviceManager>
{
    #region Editor API

    [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private string m_joinActionName = "Join";
    [SerializeField] private string m_exitActionName = "Exit";

    [SerializeField] private int m_maxPlayerCount = 2;
    [Space(8)] public UnityEvent<float> OnAllPlayerJoined;
    [Space(8)] public UnityEvent OnDelayedPlayerEvent;
    [Space(8)] public UnityEvent OnInterruptedPlayerEvent;

    [Space(8)] [SerializeField] private float m_delayBeforeAllPlayerJoinedEvent = 5.0f;

    #endregion

    #region Unity Events

    [UsedImplicitly]
    protected override void Awake()
    {
        base.Awake();
        m_connectedDevices = new List<InputDevice>(m_maxPlayerCount);
        for (int i = 0; i < m_maxPlayerCount; i++) m_connectedDevices.Add(null);
    }

    [UsedImplicitly]
    private void OnEnable()
    {
        EnableHandlers();
    }

    [UsedImplicitly]
    private void OnDisable()
    {
        DisableHandler();
    }

    #endregion

    #region API

    public int MaxPlayerCount => m_maxPlayerCount;

    public InputDevice GetDeviceByPlayerId(int playerId)
    {
        return playerId < 0 || playerId > m_maxPlayerCount ? null : m_connectedDevices[playerId];
    }

    /// <summary>
    /// Used In Scene Loader @ Start Scene
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    [UsedImplicitly]
    public void EnsureAllDevicesConnected()
    {
        int index = m_connectedDevices.IndexOf(null);
        if (index == -1) return;
        int connectedCount = m_connectedDevices.FindAll(device => device != null).Count;
        throw new System.Exception(
            $"Player count mismatch: Expected {m_connectedDevices.Count}, but only {connectedCount} devices are connected.");
    }

    public void EnableHandlers()
    {
        (InputAction joinAction, InputAction exitAction) = __M_FindActions();
        if (joinAction != null)
        {
            joinAction.performed += OnJoin;
            joinAction.Enable();
        }

        if (exitAction != null)
        {
            exitAction.performed += OnExit;
            exitAction.Enable();
        }
    }

    public void DisableHandler()
    {
        (InputAction joinAction, InputAction exitAction) = __M_FindActions();

        if (joinAction != null)
        {
            joinAction.performed -= OnJoin;
            joinAction.Disable();
        }

        if (exitAction != null)
        {
            exitAction.performed -= OnExit;
            exitAction.Disable();
        }
    }

    #endregion

    #region Event Handlers

    private void OnExit(InputAction.CallbackContext context)
    {
        InputDevice device = context.control.device;

        int index = m_connectedDevices.IndexOf(device);
        if (index == -1) return;

        m_connectedDevices[index] = null;
        --m_joinedPlayerCount;

        if (m_allPlayerJoinedCoroutine != null)
        {
            StopCoroutine(m_allPlayerJoinedCoroutine);
            OnInterruptedPlayerEvent?.Invoke();
        }

        m_allPlayerJoinedCoroutine = null;

        LevelManager.PlayerEventBus.Raise(new PlayerExitedEvent(index, device), null, gameObject);

        Debug.Log($"{device.displayName} Left! (ID: {index})");
    }

    private void OnJoin(InputAction.CallbackContext context)
    {
        InputDevice device = context.control.device;

        if (m_connectedDevices.Contains(device)) return;

        int index = m_connectedDevices.IndexOf(null);
        if (index == -1)
        {
            Debug.Log("No available slots for new devices!");
            return;
        }

        m_connectedDevices[index] = device;
        ++m_joinedPlayerCount;

        LevelManager.PlayerEventBus.Raise(new PlayerJoinedEvent(index, device), null, gameObject);

        Debug.Log($"{device.displayName} Joined! (ID: {index})");

        if (m_joinedPlayerCount == m_maxPlayerCount)
            m_allPlayerJoinedCoroutine = StartCoroutine(__M_StartInvokeAllPlayerJoinedEvent());
    }

    #endregion

    #region Internal

    private List<InputDevice> m_connectedDevices;
    private int m_joinedPlayerCount;
    private Coroutine m_allPlayerJoinedCoroutine;

    private (InputAction joinAction, InputAction exitAction) __M_FindActions()
    {
        InputAction joinAction = m_inputActionAsset.FindAction(m_joinActionName);
        InputAction exitAction = m_inputActionAsset.FindAction(m_exitActionName);

        if (joinAction == null)
        {
            throw new System.Exception($"Join action '{m_joinActionName}' not found in InputActionAsset.");
        }

        if (exitAction == null)
        {
            throw new System.Exception($"Exit action '{m_exitActionName}' not found in InputActionAsset.");
        }

        return (joinAction, exitAction);
    }

    private IEnumerator __M_StartInvokeAllPlayerJoinedEvent()
    {
        OnAllPlayerJoined?.Invoke(m_delayBeforeAllPlayerJoinedEvent);
        yield return new WaitForSecondsRealtime(m_delayBeforeAllPlayerJoinedEvent);
        OnDelayedPlayerEvent?.Invoke();
    }

    #endregion
}