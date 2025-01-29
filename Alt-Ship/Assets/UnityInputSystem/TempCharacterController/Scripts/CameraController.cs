using Cinemachine;
using EE.AMVCC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;

public class CameraController : MonoBehaviour
{
    private List<PlayerInput> players = new();

    [SerializeField] private List<LayerMask> playerLayers;

    public PlayerInputManager playerInputManager;

    private InputActionAsset inputAsset;
    private InputActionMap inputMap;

    public Transform spawnPointA;
    public Transform spawnPointB;

    public GameObject shipModel;

    private int m_playerCount;

    [Header("Refs.")]
    // TODO: Refactor using device manager
    [SerializeField]
    private DirectionalInteraction m_interaction;

    public int PlayerCount
    {
        get => m_playerCount;
        set => m_playerCount = value;
    }

    private void OnEnable()
    {
        // playerInputManager.onPlayerJoined += AddPlayer;
        // playerInputManager.onPlayerJoined += SpawnLocation;
        playerInputManager.onPlayerLeft += __M_CleanUp;
        m_playerCount += 1;
    }

    private void OnDisable()
    {
        // playerInputManager.onPlayerJoined -= AddPlayer;
        // playerInputManager.onPlayerJoined -= SpawnLocation;
        playerInputManager.onPlayerLeft -= __M_CleanUp;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        inputAsset = player.GetComponent<PlayerInput>().actions;

        inputMap = inputAsset.FindActionMap("PlayerMovement");

        Transform playerParent = player.transform;

        playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        playerParent.GetComponentInChildren<CameraInput>().horizontal = inputMap.FindAction("Camera");

        var devices = player.devices;

        if (devices.Count == 0)
        {
            Debug.LogWarning($"{player.gameObject.name} joined, but no input devices were found.");
        }
        else
        {
            var deviceInfo = string.Join(", ",
                devices.Select(device => $"[Name: {device.displayName}, ID: {device.deviceId}]"));
            Debug.Log($"{player.gameObject.name} joined with the following devices: {deviceInfo}");
        }

        InputDevice inputDevice = devices.Count > 0 ? devices[0] : null;

        m_interaction.RegisterPlayer(player.gameObject, inputDevice);
        Application.Instance.RegisterController(player.GetComponent<IController>());
    }

    private void __M_CleanUp(PlayerInput player)
    {
        --m_playerCount;

        if (Application.Instance != null) Application.Instance.UnregisterController(player.GetComponent<IController>());
    }

    public void SpawnLocation(PlayerInput player)
    {
        player.transform.parent = shipModel.transform;
        if (spawnPointA != null && spawnPointB != null)
        {
            if (m_playerCount == 1)
            {
                player.transform.position = spawnPointA.transform.position;
                player.transform.rotation = shipModel.transform.rotation;
            }
            else if (m_playerCount == 2)
            {
                player.transform.position = spawnPointB.transform.position;
                player.transform.rotation = shipModel.transform.rotation;
            }
        }
        else
        {
        }

        if (shipModel != null)
        {
            player.transform.parent = shipModel.transform;
        }
    }
}