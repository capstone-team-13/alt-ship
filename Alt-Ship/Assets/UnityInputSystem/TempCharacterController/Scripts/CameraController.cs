using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField]
    private List<LayerMask> playerLayers;

    public PlayerInputManager playerInputManager;

    private InputActionAsset inputAsset;
    private InputActionMap inputMap;

    public Transform spawnPointA;
    public Transform spawnPointB;

    public GameObject shipModel;

    int playerNumber = 0;

    [Header("Refs.")]
    // TODO: Refactor using device manager
    [SerializeField] private DirectionalInteraction m_interaction;
    
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerJoined += SpawnLocation;
        playerNumber += 1;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerLeft -= AddPlayer;
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
    }

    public void SpawnLocation(PlayerInput player)
    {
        player.transform.parent = shipModel.transform;
        if (spawnPointA != null && spawnPointB != null)
        {
            if (playerNumber == 1)
            {
                player.transform.position = spawnPointA.transform.position;

            }
            else if (playerNumber == 2)
            {
                player.transform.position = spawnPointB.transform.position;
            }
        }
        else
        {
        }
        if(shipModel != null)
        {
            player.transform.parent = shipModel.transform;
        }

    }

}