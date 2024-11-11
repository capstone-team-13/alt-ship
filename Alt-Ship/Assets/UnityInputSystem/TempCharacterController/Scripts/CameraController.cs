using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
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


    private void OnEnable()
    {
        Debug.Log("Hello world 1");
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerJoined += SpawnLocation;
    }

    private void OnDisable()
    {
        Debug.Log("Hello world 2");
        playerInputManager.onPlayerLeft -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        Debug.Log("Hello world");
        players.Add(player);
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        inputAsset = player.GetComponent<PlayerInput>().actions;
        
        inputMap = inputAsset.FindActionMap("PlayerMovement");

        Transform playerParent = player.transform; 

        playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        playerParent.GetComponentInChildren<CameraInput>().horizontal = inputMap.FindAction("Camera");


    }

    public void SpawnLocation(PlayerInput player)
    {
        if (spawnPointA != null && spawnPointB != null)
        {
            if (PlayerCounter.playerCount == 1)
            {
                player.transform.position = spawnPointA.transform.position;
            }
            else if (PlayerCounter.playerCount == 2)
            {
                player.transform.position = spawnPointB.transform.position;
            }
        }
    }

}