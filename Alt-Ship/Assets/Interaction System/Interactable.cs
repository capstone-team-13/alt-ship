using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace EE.Interactions
{
    public interface IInteractable
    {
        string KeyName { get; }
        string InteractionName { get; set; }
    }

    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        private bool m_canInteract = true;
        private double m_lastInteractTime;

        #region Editor API

        [Header("Configs")] [SerializeField] private string m_interactionName;

        [SerializeField] [Space(4)] private InputAction m_inputAction;
        [SerializeField] [Space(4)] private float m_interactionCoolingDown = 2.0f;

        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnActivated;

        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnInteracted;

        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnDeactivate;

        #endregion

        #region API

        public string KeyName { get; private set; }

        public string InteractionName
        {
            get => m_interactionName;
            set => m_interactionName = value;
        }

        protected virtual bool CanInteract()
        {
            var notInCoolingDown = Time.time - m_lastInteractTime > m_interactionCoolingDown;
            return notInCoolingDown;
        }

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void Awake()
        {
            m_lastInteractTime = Time.time - m_interactionCoolingDown;
            __M_UpdateKeyName();
        }

        [UsedImplicitly]
        private void Update()
        {
            // 'Triggered' indicates that the action was either pressed or released.
            // We need to verify that the key is actually pressed in this context.
            var desiredKeyPressed = m_inputAction is { triggered: true }
                                    && m_inputAction.activeControl?.IsPressed() == true;

            var canInteract = CanInteract();
            if (canInteract)
            {
                OnActivated?.Invoke(this);
                if (desiredKeyPressed) __M_Interact();
            }
            // If interaction is not possible this frame but was possible in the previous frame.
            else if (m_canInteract) OnDeactivate?.Invoke(this);

            m_canInteract = canInteract;
        }

        [UsedImplicitly]
        protected virtual void OnEnable()
        {
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

        private void __M_Interact()
        {
            m_lastInteractTime = Time.time;
            OnInteracted?.Invoke(this);
        }

        private void __M_OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change != InputDeviceChange.Added && change != InputDeviceChange.ConfigurationChanged) return;
            Debug.Log("Device changed: " + device.displayName);

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

        #endregion
    }
}