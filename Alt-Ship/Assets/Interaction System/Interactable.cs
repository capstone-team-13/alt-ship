using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace EE.Interactions
{
    public interface IInteractable
    {
        KeyCode KeyCode { get; }
        string InteractionName { get; set; }
    }

    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        private enum DeviceType
        {
            Keyboard = 0,
            XboxController,
        }

        private static DeviceType m_deviceType = DeviceType.Keyboard;

        private bool m_canInteract = true;
        private double m_lastInteractTime;

        #region Editor API

        [Header("Configs")] [SerializeField] [Tooltip("0 - Keyboard, 1 - Xbox Controller")]
        private KeyCode[] m_interactionKey = { KeyCode.E, KeyCode.Joystick1Button2 };

        [SerializeField] private string m_interactionName;

        [SerializeField] [Space(4)] [Tooltip("In second (s)")]
        private float m_interactionCoolingDown;


        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnActivated;


        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnInteracted;

        [Space(4)] [Tooltip("Assign your callbacks here")]
        public UnityEvent<IInteractable> OnDeactivate;

        #endregion

        #region API

        public KeyCode KeyCode => m_interactionKey[(int)m_deviceType];

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
        }

        [UsedImplicitly]
        private void Update()
        {
            var desiredKeyPressed = Input.GetKeyDown(m_interactionKey[(int)m_deviceType]);

            var canInteract = CanInteract();
            if (canInteract)
            {
                OnActivated?.Invoke(this);
                if (desiredKeyPressed) __M_Interact();
            }
            else if (m_canInteract) OnDeactivate?.Invoke(this);

            m_canInteract = canInteract;
        }

        #endregion

        #region Internal

        private void __M_Interact()
        {
            m_lastInteractTime = Time.time;
            OnInteracted?.Invoke(this);
        }

        #endregion
    }
}