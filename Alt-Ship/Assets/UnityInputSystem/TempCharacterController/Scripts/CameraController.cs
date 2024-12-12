using Cinemachine;
using EE.AMVCC;
using System.Collections.Generic;
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

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerJoined += SpawnLocation;
        playerInputManager.onPlayerLeft += __M_CleanUp;
        m_playerCount += 1;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerJoined -= SpawnLocation;
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

        Debug.Log($"{player.gameObject.name} joined with device {player.GetDevice<InputDevice>().deviceId}");

        m_interaction.RegisterPlayer(player.gameObject, player.GetDevice<InputDevice>());
        Application.Instance.RegisterController(player.GetComponent<IController>());
    }

    private void __M_CleanUp(PlayerInput player)
    {
        --m_playerCount;
        Application.Instance.RegisterController(player.GetComponent<IController>());
    }

    public void SpawnLocation(PlayerInput player)
    {
        player.transform.parent = shipModel.transform;
        if (spawnPointA != null && spawnPointB != null)
        {
            var playerController = player.GetComponent<PlayerController>();
            playerController.UpdatePlayerId();

            switch (m_playerCount)
            {
                case 1:
                    player.transform.position = spawnPointA.position;
                    player.transform.rotation = shipModel.transform.rotation;
                    playerController.SetTargetPositionXZ(spawnPointA.position);
                    break;

                case 2:
                    player.transform.position = spawnPointB.position;
                    player.transform.rotation = shipModel.transform.rotation;
                    playerController.SetTargetPositionXZ(spawnPointB.position);
                    break;
            }
        }

        if (shipModel != null)
        {
            player.transform.parent = shipModel.transform;
        }
    }
}