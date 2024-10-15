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

    private PlayerInputManager playerInputManager;


    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerLeft -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);
        Transform playerParent = player.transform.parent;

        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        playerParent.GetComponentInChildren<CinemachineInputProvider>().PlayerIndex = players.Count;
        Debug.Log(players.Count);
        playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
        playerParent.GetComponentInChildren<CameraInput>().horizontal = player.actions.FindAction("Camera");


    }

}