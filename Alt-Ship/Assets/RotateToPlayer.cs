using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class RotateToPlayer : MonoBehaviour
{
    // True = One || False = Two
    [SerializeField] private bool playerChosen;
    [SerializeField] private LayerMask m_playerLayer;
    private GameObject playerModel;
    private CinemachineFreeLook cinemachineFreeLook;


    private void FixedUpdate()
    {
        ButtonPrompts();
    }

    private void ButtonPrompts()
    {
        var colliders = Physics.OverlapSphere(transform.position, 100f, m_playerLayer);
        HashSet<PlayerController> currentPlayers = new HashSet<PlayerController>();

        foreach (var agent in colliders)
        {
            PlayerController playerController = agent.gameObject.GetComponent<PlayerController>();
            if (playerController == null) continue;

            currentPlayers.Add(playerController);

            RotateToCamera(playerController.playerFreeLook, playerController);
        }

    }

    private void RotateToCamera(CinemachineFreeLook playerCam, PlayerController playerController)
    {
        if (playerController.playerNum == 1 && playerChosen)
        {
            Vector3 directionToCamera = playerCam.transform.position - this.transform.position;
            directionToCamera.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            this.transform.rotation =
            Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else if(playerController.playerNum == 2 && !playerChosen)
        {
            Vector3 directionToCamera = playerCam.transform.position - this.transform.position;
            directionToCamera.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            this.transform.rotation =
            Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

}
