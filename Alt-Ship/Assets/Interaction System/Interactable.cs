using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace EE.Interactions
{
    public class Interactable : MonoBehaviour
    {
        private enum DeviceType
        {
            Keyboard = 0,
            XboxController,
        }

        private static DeviceType m_deviceType = DeviceType.Keyboard;

        private bool m_canInteractAFrameAgo = true;
        private bool m_canInteract = true;
        private double m_lastInteractTime;

        [Header("Configs")] [SerializeField] [Tooltip("0 - Keyboard, 1 - Xbox Controller")]
        private KeyCode[] m_interactionKey = { KeyCode.E, KeyCode.Joystick1Button2 };

        [SerializeField] private string m_interactionName;

        [SerializeField] [Space(4)] [Tooltip("In second (s)")]
        private float m_interactionCoolingDown;


        #region API

        public KeyCode KeyCode => m_interactionKey[(int)m_deviceType];

        public string InteractionName
        {
            get => m_interactionName;
            set => m_interactionName = value;
        }


        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<Interactable> OnActivated;


        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<Interactable> OnInteracted;

        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<Interactable> OnDeactivate;


        [Space(4)] [Tooltip("Interaction Condition")]
        public Func<bool> CanInteract;

        #endregion

        private void Awake()
        {
            m_lastInteractTime = Time.time - m_interactionCoolingDown;
        }

        [UsedImplicitly]
        private void Update()
        {
            var desiredKeyPressed = Input.GetKeyDown(m_interactionKey[(int)m_deviceType]);
            m_canInteract = Time.time - m_lastInteractTime > m_interactionCoolingDown;
#if UNITY_EDITOR
            if (desiredKeyPressed && !m_canInteract)
            {
                Debug.Log($"Interaction is cooling down for {gameObject.name}.");
            }
#endif
            // TODO: UI Implementation
            // TODO: Better State Management
            var canInteract = CanInteract() && m_canInteract;
            if (canInteract)
                OnActivated?.Invoke(this);
            if (m_canInteractAFrameAgo && !canInteract)
                OnDeactivate?.Invoke(this);

            if (canInteract && desiredKeyPressed)
                __M_Interact();

            m_canInteractAFrameAgo = canInteract;
        }

        private void __M_Interact()
        {
            m_lastInteractTime = Time.time;
            OnInteracted?.Invoke(this);
        }
    }
}