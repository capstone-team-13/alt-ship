using JetBrains.Annotations;
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

        private bool m_canInteract = true;
        private double m_lastInteractTime;

        [Header("Configs")]
        [SerializeField] [Tooltip("0 - Keyboard, 1 - Xbox Controller")]private KeyCode[] m_interactionKey = { KeyCode.E, KeyCode.Joystick1Button2 };
        [SerializeField][Space(4)][Tooltip("In second (s)")] private float m_interactionCoolingDown;
        [Space(4)][Tooltip("Assign your callbacks here")] public UnityEvent<GameObject> OnInteracted;

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
            if (desiredKeyPressed && m_canInteract) __M_Interact();
        }

        private void __M_Interact()
        {
            m_lastInteractTime = Time.time;
            OnInteracted?.Invoke(gameObject);
        }
    }
}
