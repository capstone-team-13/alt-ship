using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace EE.Interactions
{
    public interface IInteractable
    {
        string KeyName { get; }
        string InteractionName { get; set; }
    }

    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        #region Editor API
        [Header("Button Prompts")]
        public GameObject pOneButton;
        public GameObject pTwoButton;
        public GameObject uiControlsOne;
        public GameObject uiControlsTwo;

        [Header("Configs")][SerializeField] private string m_interactionName;

        [SerializeField][Space(4)] private InputAction m_inputAction;
        [SerializeField][Space(4)] private float m_interactionCoolingDown = 2.0f;

        [Space(4)]
        [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnActivated;

        [Space(4)]
        [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable, GameObject> OnInteracted;

        // TODO: Another callback handles player exit;
        [Space(4)]
        [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable, GameObject> OnExit;

        [Space(4)]
        [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnDeactivate;

        #endregion

        #region API

        public string KeyName { get; private set; }

        public string InteractionName
        {
            get => m_interactionName;
            set => m_interactionName = value;
        }

        protected GameObject CurrentPlayer
        {
            get => m_currentPlayer;
            set => m_currentPlayer = value;
        }


        protected GameObject CurrentPerformer
        {
            get => m_currerntPerformer;
            set => m_currerntPerformer = value;
        }


        protected virtual bool CanInteract()
        {
            var notInCoolingDown = Time.time - m_lastInteractTime 
                                   > m_interactionCoolingDown;

            // Reset current player to null if interacting with the same player again
            // This replaces the check for current player not equal to performing player   

            var otherPlayerNotInUse = m_currentPlayer == null;
            return notInCoolingDown;
            // return notInCoolingDown && otherPlayerNotInUse;
        }

        public void RegisterPlayer(GameObject player, InputDevice device)
        {
            m_devicePlayerMap[device.deviceId] = player;
        }

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void Awake()
        {
            m_lastInteractTime = Time.time - m_interactionCoolingDown;
            __M_UpdateKeyName();

            m_inputAction.performed += __M_TryInteract;
        }

        [UsedImplicitly]
        private void Update()
        {
            var canInteract = CanInteract();
            if (canInteract) OnActivated?.Invoke(this);
            // If interaction is not possible this frame but was possible in the previous frame.
            else if (m_canInteract) OnDeactivate?.Invoke(this);

            m_canInteract = canInteract;
        }

        [UsedImplicitly]
        protected virtual void OnEnable()
        {
            OnExit.AddListener(__M_Reset);

            m_inputAction?.Enable();

            InputSystem.onDeviceChange += __M_OnDeviceChange;
        }

        [UsedImplicitly]
        protected virtual void OnDisable()
        {
            m_inputAction?.Disable();

            InputSystem.onDeviceChange -= __M_OnDeviceChange;
        }

        #endregion

        #region Internal

        private bool m_canInteract = true;
        private double m_lastInteractTime;
        private GameObject m_currentPlayer;
        private GameObject m_currerntPerformer;

        // TODO: Refactor to Device Manager
        private static Dictionary<int, GameObject> m_devicePlayerMap = new();

        // TODO: Refactor exit condition
        private void __M_TryInteract(CallbackContext context)
        {
            var deviceId = context.control.device.deviceId;
            // Reset interaction state if the same user triggered it.
            CurrentPerformer = m_devicePlayerMap[deviceId];

            // TODO: Seperate RangeInteraction and Directional Interactable
            // Reset the current player to null if it is the performed player, allowing re-interaction
            if (CurrentPlayer == CurrentPerformer) CurrentPlayer = null;

            // 'Triggered' indicates that the action was either pressed or released.
            // We need to verify that the key is actually pressed in this context.
            var desiredKeyPressed = m_inputAction is { triggered: true }
                                    && m_inputAction.activeControl?.IsPressed() == true;

            var canInteract = CanInteract();
            if (canInteract)
            {
                bool isDifferentFromInteractor = CurrentPlayer != null && CurrentPlayer != CurrentPerformer;
                if (isDifferentFromInteractor) OnExit?.Invoke(this, CurrentPlayer);

                OnActivated?.Invoke(this);
                if (desiredKeyPressed) __M_Interact(CurrentPerformer);
            }
            // TODO: Potential bug
            // If interaction is not possible this frame but was possible in the previous frame.
            else if (m_canInteract) OnDeactivate?.Invoke(this);

            m_canInteract = canInteract;
        }

        private void __M_Interact(GameObject interactor)
        {
            m_lastInteractTime = Time.time;
            m_currentPlayer = interactor;
            OnInteracted?.Invoke(this, interactor);
        }

        private void __M_OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            // TODO: Refactor to Device Manager
            // If device disconnected remove it
            if (change == InputDeviceChange.Removed && m_devicePlayerMap.TryGetValue(device.deviceId, out _))
                m_devicePlayerMap.Remove(device.deviceId);

            if (change != InputDeviceChange.Added && change != InputDeviceChange.ConfigurationChanged) return;
            if (m_inputAction == null) return;

            __M_UpdateKeyName();
        }

        private void __M_UpdateKeyName()
        {
            foreach (var binding in m_inputAction.bindings)
            {
                var path = binding.effectivePath;
                var control = InputSystem.FindControl(path);

                if (control != null) KeyName = control.displayName;
            }
        }

        private void __M_Reset(IInteractable interactable, GameObject interactor)
        {
            Debug.Log("@Reset");
            CurrentPerformer = null;
            CurrentPlayer = null;
        }

        #endregion
    }
}