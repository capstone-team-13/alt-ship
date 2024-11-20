using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;
using Cinemachine;
using System;

public class SailFunction : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction sailing;

    private bool m_sailing = false;

    public float sailPositionSpeed = 0.1f;

    public ShipModel shipModel;

    private bool isRaising;
    private bool isLowering;


    private void Awake()
    {
        var actionMap = inputActions.FindActionMap("Sail");
        sailing = actionMap.FindAction("Pulling");
    }

    private void Update()
    {
        if (!m_sailing) return;

        if (isLowering)
        {
            shipModel.Speed += 1f * Time.deltaTime;
        }
        else if (isRaising)
        {
            shipModel.Speed -= 1f * Time.deltaTime;
        }
        else
        {
            return;
        }
    }


    public void Interact(IInteractable interactable, GameObject interactor)
    {

        sailing.Enable();
        sailing.performed += PullingSail;
        sailing.canceled += NotPullingSail;

        m_sailing = !m_sailing;
        interactable.InteractionName = m_sailing ? "Stop Interacting" : "Start Interacting";

        var playerModel = interactor.GetComponent<PlayerModel>();

        if (m_sailing) __M_LockPlayer(playerModel);
        else __M_UnLockPlayer(playerModel);
    }

    // Exit is never running
    public void Exit(IInteractable interactable, GameObject interactor)
    {
        interactable.InteractionName = "Stop Interacting";
        var playeModel = interactor.GetComponent<PlayerModel>();
        __M_UnLockPlayer(playeModel);
    }

    private void __M_LockPlayer(PlayerModel player)
    {
        const float newSpeed = 0;
        Application.Instance.Push(new PlayerCommand.ChangeSpeed(player, newSpeed));
        Debug.Log($"<color=#FFFF00>{player.name} Locked.</color>");
    }

    private void __M_UnLockPlayer(PlayerModel player)
    {
        sailing.performed -= PullingSail;
        sailing.canceled -= NotPullingSail;
        sailing.Disable();
        m_sailing = false;

        // TODO: Reference Regular Speed
        const float newSpeed = 5;
        Application.Instance.Push(new PlayerCommand.ChangeSpeed(player, newSpeed));
        Debug.Log($"<color=#FFFF00>{player.name} Unlocked.</color>");
    }

    private void PullingSail(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() > 0)
        {
            isRaising = true;
            isLowering = false;

        }
        else if(context.ReadValue<float>() < 0)
        {
            isLowering = true;
            isRaising = false;
        }
        else
        {
            isRaising = false;
            isLowering = false;
        }
    }
    private void NotPullingSail(InputAction.CallbackContext context)
    {
        isRaising = false;
        isLowering = false;
    }

}


