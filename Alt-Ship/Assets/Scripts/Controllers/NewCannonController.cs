using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;
using Cinemachine;
using System;
using EE.AMVCC;

public class NewCannonController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction firing;
    private InputAction tilting;
    private PlayerInput playerInput;

    public CinemachineVirtualCamera cannonCam1;
    public CinemachineVirtualCamera cannonCam2;
    private CinemachineFreeLook lastPlayerCam;

    private Transform playerTransform;
    private Vector3 recentPosition;

    public Rigidbody cannonBall;
    public GameObject shootAt;

    [Range(0f, 50f)]
    public float fireForce;

    [Range(-7f, 7f)]
    public float zRotation;

    [Range(0f, 5f)]
    public float rotateRate;

    private bool isRaising;
    private bool isLowering;

    public Transform barrelTransform;
    
    private bool m_manning = false;

    private void Update()
    {
        if (!m_manning) return;

        if (isRaising)
        {
            zRotation += rotateRate * Time.deltaTime;
        }
        else if (isLowering)
        {
            zRotation -= rotateRate * Time.deltaTime;
        }
        zRotation = Mathf.Clamp(zRotation, -7, 7);
        barrelTransform.eulerAngles = new Vector3(barrelTransform.eulerAngles.x, barrelTransform.eulerAngles.y, zRotation);

        if (playerTransform != null && playerTransform.position != recentPosition)
        {
            playerTransform.localPosition = recentPosition;
        }
    }


    public void Interact(IInteractable interactable, GameObject interactor)
    {
        playerInput = interactor.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            inputActions = playerInput.actions;
            NewPlayerStarted();
        }

        m_manning = !m_manning;
        interactable.InteractionName = m_manning ? "Stop Interacting" : "Start Interacting";

        var playerModel = interactor.GetComponent<PlayerModel>();

        if (m_manning) __M_LockPlayer(playerModel);
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
            cannonCam1.Priority = 10;
        }
        else if (lastPlayerCam != null && playerNum == 2)
        {
            lastPlayerCam.Priority = 5;
            cannonCam2.Priority = 10;
        }

        const float newSpeed = 0;
        Application.Instance.Push(new PlayerCommand.ChangeSpeed(player, newSpeed));
        Debug.Log($"<color=#FFFF00>{player.name} Locked.</color>");
    }

    private void __M_UnLockPlayer(PlayerModel player)
    {

        m_manning = false;

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
            cannonCam1.Priority = 5;
        }
        else if (lastPlayerCam != null && playerNum == 2)
        {
            lastPlayerCam.Priority = 10;
            cannonCam2.Priority = 5;
        }

        playerTransform = null;
        lastPlayerCam = null;

        StationAbandonded();

        // TODO: Reference Regular Speed
        const float newSpeed = 5;
        Application.Instance.Push(new PlayerCommand.ChangeSpeed(player, newSpeed));
        Debug.Log($"<color=#FFFF00>{player.name} Unlocked.</color>");
    }

    private void FiringCannon(InputAction.CallbackContext context)
    {
        Debug.Log("Cannon Setup");
        Rigidbody launchedBall = Instantiate(cannonBall, shootAt.transform.position, Quaternion.identity);
        launchedBall.AddForce(shootAt.transform.forward * fireForce, ForceMode.Impulse);
    }

    private void TiltingCannon(InputAction.CallbackContext context)
    {

        if (context.ReadValue<float>() > 0)
        {
            isRaising = true;
            isLowering = false;

        }
        else if (context.ReadValue<float>() < 0)
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

    private void stopTilting(InputAction.CallbackContext context)
    {
        isRaising = false;
        isLowering = false;
    }

    private void NewPlayerStarted()
    {
        Debug.Log("Cannon Setup");
        var actionMap = inputActions.FindActionMap("CannonFire");
        firing = actionMap.FindAction("Fire");
        tilting = actionMap.FindAction("Tilt");
        tilting.Enable();
        firing.Enable();

        firing.performed += FiringCannon;
        tilting.performed += TiltingCannon;
        tilting.canceled += stopTilting;
    }

    private void StationAbandonded()
    {
        firing.performed -= FiringCannon;
        tilting.performed -= TiltingCannon;
        tilting.canceled -= TiltingCannon;

        firing.Disable();
        tilting.Disable();

        playerInput = null;
        inputActions = null;
    }

}
