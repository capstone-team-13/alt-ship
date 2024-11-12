using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;
using Application = EE.AMVCC.Application;
using UnityEngine.InputSystem;

public class Rudder : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction steering;


    private bool m_steering;
    private float rotationSign;

    private void Awake()
    {
        var actionMap = inputActions.FindActionMap("Steering");
        steering = actionMap.FindAction("ShipSteering");
    }

    void OnEnable()
    {
        rotationSign = 0.0f;
        steering.Enable();
        steering.performed += steeringBoat;
        steering.canceled += steeringCanceled;
    }

    private void OnDisable()
    {
        steering.performed -= steeringBoat;
        steering.canceled -= steeringCanceled;
        rotationSign = 0.0f;
        steering.Disable();

    }


    #region Unity Callbacks

    [UsedImplicitly]
    private void Update()
    {
        if (!m_steering) return;

        // TODO: Refactor using new input system

        if (rotationSign != 0f)
        {
            Debug.Log("Steering");
            Application.Instance.Push(new ShipCommand.Steer(rotationSign));
        }
    }

    private void steeringBoat(InputAction.CallbackContext context)
    {
        rotationSign = context.ReadValue<float>();
    }

    private void steeringCanceled(InputAction.CallbackContext context)
    {
        rotationSign = 0.0f;
    }


    #endregion

    #region API

    public void Interact(IInteractable interactable)
    {
        m_steering = !m_steering;
        interactable.InteractionName = m_steering ? "Stop Steering" : "Start Steer";
    }

    #endregion
}