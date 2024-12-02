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

    [Range(0f, 20f)]
    public float maxSpeed;

    public ShipModel shipModel;

    private bool isRaising;
    private bool isLowering;

    public CinemachineVirtualCamera sailCam1;
    public CinemachineVirtualCamera sailCam2;
    private CinemachineFreeLook lastPlayerCam;

    private Transform playerTransform;
    private Vector3 recentPosition;

    public LineRenderer sailIndicator;

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
        shipModel.Speed = Mathf.Clamp(shipModel.Speed, 0f, maxSpeed);
        sailIndicator.SetPosition(1, new Vector3(0, shipModel.Speed * -1, 0));

        if (playerTransform != null && playerTransform.position != recentPosition)
        {
            playerTransform.localPosition = recentPosition;
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
        if (playerTransform == null)
        {
            playerTransform = player.transform;
            recentPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z);
        }

        float playerNum = 0;

        if (player.transform.GetComponent<PlayerController>() != null)
        {
            playerNum = player.transform.GetComponent<PlayerController>().playerNum;
        }

        lastPlayerCam = player.GetComponentInChildren<CinemachineFreeLook>();

        if (lastPlayerCam != null && playerNum == 1)
        {
        //    Debug.Log("Has loaded");
            lastPlayerCam.Priority = 5;
            sailCam1.Priority = 10;
        }
        else if(lastPlayerCam != null && playerNum == 2)
        {
            lastPlayerCam.Priority = 5;
            sailCam2.Priority = 10;
        }

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

        float playerNum = 0;

        if (player.transform.GetComponent<PlayerController>() != null)
        {
            playerNum = player.transform.GetComponent<PlayerController>().playerNum;
        }

        lastPlayerCam = player.GetComponentInChildren<CinemachineFreeLook>();

        if (lastPlayerCam != null && playerNum == 1)
        {
        //    Debug.Log("Has loaded");
            lastPlayerCam.Priority = 10;
            sailCam1.Priority = 5;
        }
        else if (lastPlayerCam != null && playerNum == 2)
        {
            lastPlayerCam.Priority = 10;
            sailCam2.Priority = 5;
        }

        playerTransform = null;
        lastPlayerCam = null;

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


