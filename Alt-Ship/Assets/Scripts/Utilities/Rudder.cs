using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;
using Cinemachine;
using UnityEngine.Rendering;
public class Rudder : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction steering;

    private PlayerInput playerInput;

    public CinemachineVirtualCamera rudCam1;
    public CinemachineVirtualCamera rudCam2;
    private CinemachineFreeLook lastPlayerCam;

    [SerializeField] private GameObject pOneSail;
    [SerializeField] private GameObject pTwoSail;

    [SerializeField] private Material standard;
    [SerializeField] private Material transparent;

    // TODO: Temp solution
    [SerializeField] private DirectionalInteraction m_interactable;

    private Transform playerTransform;

    private Vector3 recentPosition;

    private bool m_steering;
    private float rotationSign;

    #region Unity Callbacks

    [UsedImplicitly]
    private void Update()
    {
        if (!m_steering) return;

        // TODO: Refactor using new input system

        if (rotationSign != 0f)
        {
            //          Debug.Log("Steering");
            Application.Instance.Push(new ShipCommand.Steer(rotationSign));
        }

        if (playerTransform != null && playerTransform.position != recentPosition)
        {
            playerTransform.localPosition = recentPosition;
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

    public void Interact(IInteractable interactable, GameObject interactor)
    {
        playerInput = interactor.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            inputActions = playerInput.actions;
            NewPlayerStarted();
        }

        m_steering = !m_steering;
        interactable.InteractionName = m_steering ? "Stop Steering" : "Start Steer";

        var playerModel = interactor.GetComponent<PlayerModel>();

        if (m_steering) __M_LockPlayer(playerModel);
        else
        {
            __M_UnLockPlayer(playerModel);
            m_interactable.__M_Reset();
        }

    }

    // Exit is never running
    public void Exit(IInteractable interactable, GameObject interactor)
    {
        Debug.Log("Is Running");
        interactable.InteractionName = "Stop Steering";

        var playeModel = interactor.GetComponent<PlayerModel>();
        __M_UnLockPlayer(playeModel);
    }

    #endregion

    #region Internal

    private void __M_LockPlayer(PlayerModel player)
    {
        if (playerTransform == null)
        {
            playerTransform = player.transform;
            recentPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z);
        }

        // Camera Start
        float playerNum = 0;
        if (player.transform.GetComponent<PlayerController>() != null)
        {
            playerNum = player.transform.GetComponent<PlayerController>().playerNum;
        }

        lastPlayerCam = player.GetComponentInChildren<CinemachineFreeLook>();

        if (lastPlayerCam != null && playerNum == 1)
        {
            lastPlayerCam.Priority = 5;
            rudCam1.Priority = 10;
            //
            if (pOneSail.GetComponentInChildren<MeshRenderer>().material)
            {
                MeshRenderer materialTest = pOneSail.GetComponentInChildren<MeshRenderer>();
                materialTest.material = transparent;
            }
        }
        else if (lastPlayerCam != null && playerNum == 2)
        {
            lastPlayerCam.Priority = 5;
            rudCam2.Priority = 10;
            //
            if (pTwoSail.GetComponentInChildren<MeshRenderer>().material)
            {
                MeshRenderer materialTest = pTwoSail.GetComponentInChildren<MeshRenderer>();
                materialTest.material = transparent;
            }
        }
        // Camera End

        const float newSpeed = 0;
        Application.Instance.Push(new PlayerCommand.ChangeSpeed(player, newSpeed));
        Debug.Log($"<color=#FFFF00>{player.name} Locked.</color>");
    }

    private void __M_UnLockPlayer(PlayerModel player)
    {
        Debug.Log("Rudder Player has Unlocked Start");

        m_steering = false;

        // Camera Start
        float playerNum = 0;
        if (player.transform.GetComponent<PlayerController>() != null)
        {
            playerNum = player.transform.GetComponent<PlayerController>().playerNum;
        }

        lastPlayerCam = player.GetComponentInChildren<CinemachineFreeLook>();

        if (lastPlayerCam != null && playerNum == 1)
        {
            lastPlayerCam.Priority = 10;
            rudCam1.Priority = 5;
            //
            if (pOneSail.GetComponentInChildren<MeshRenderer>().material)
            {
                MeshRenderer materialTest = pOneSail.GetComponentInChildren<MeshRenderer>();
                materialTest.material = standard;
            }
        }
        else if (lastPlayerCam != null && playerNum == 2)
        {
            lastPlayerCam.Priority = 10;
            rudCam2.Priority = 5;
            //
            if (pTwoSail.GetComponentInChildren<MeshRenderer>().material)
            {
                MeshRenderer materialTest = pTwoSail.GetComponentInChildren<MeshRenderer>();
                materialTest.material = standard;
            }
        }
        lastPlayerCam = null;
        playerTransform = null;
        // Camera End

        StationAbandonded();

        // TODO: Reference Regular Speed
        const float newSpeed = 5;
        Application.Instance.Push(new PlayerCommand.ChangeSpeed(player, newSpeed));
        // TODO: Remove
        Debug.Log($"<color=#FFFF00>{player.name} Unlocked.</color>");
        Debug.Log("Rudder Player has Unlocked Finish");
    }

    #endregion

    private void NewPlayerStarted()
    {
        var actionMap = inputActions.FindActionMap("Steering");
        steering = actionMap.FindAction("ShipSteering");

        rotationSign = 0.0f;
        steering.Enable();
        steering.performed += steeringBoat;
        steering.canceled += steeringCanceled;
    }

    private void StationAbandonded()
    {
        steering.performed -= steeringBoat;
        steering.canceled -= steeringCanceled;
        rotationSign = 0.0f;
        steering.Disable();

        playerInput = null;
        inputActions = null;
    }


}