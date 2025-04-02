using Cinemachine;
using EE.Interactions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirectionalInteraction : Interactable
{
    #region Editor API

    [Header("Directional Interaction")] [SerializeField]
    private float m_interactionRadius = 2.0f;

    [SerializeField] [Range(-180, 180)] private float m_imageRotationY = 0;

    [SerializeField] private LayerMask m_playerLayer;

    private HashSet<PlayerController> previousPlayers = new HashSet<PlayerController>();

    #endregion

    #region Unity Callbacks

    [UsedImplicitly]
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_interactionRadius);

        Gizmos.color = Color.yellow;
        var rotatedVector = __M_RotateImageForward();
        Gizmos.DrawRay(transform.position, rotatedVector * 2.0f);
    }

    private void FixedUpdate()
    {
        ButtonPrompts();
    }

    #endregion

    #region API

    // Now it's the same as range interaction
    protected override bool CanInteract()
    {
        // var notInCoolingDown = base.CanInteract();
        // var colliders = Physics.OverlapSphere(transform.position, m_interactionRadius, m_playerLayer);
        // return notInCoolingDown && colliders.Length > 0;

        // Operation is not in cooling down & no player is using object
        var baseCheck = base.CanInteract();
        if (!baseCheck) return false;
        
        var colliders = Physics.OverlapSphere(transform.position, m_interactionRadius, m_playerLayer);
        
        var rotatedForward = __M_RotateImageForward();
        
        // TODO: Refactor to specific player can interact
        var anyAgentBehind = false;
        
        foreach (var agent in colliders)
        {
            var directionToAgent = agent.transform.position - transform.position;
            var dotProduct = Vector3.Dot(rotatedForward, directionToAgent.normalized);
            var behindObject = dotProduct < 0;
        
            if (!behindObject || agent.gameObject != CurrentPerformer)
                continue;
        
            anyAgentBehind = true;
            break;
        }
        
        return anyAgentBehind;
    }

    #endregion

    #region Internal

    public Vector3 __M_RotateImageForward()
    {
        var rotation = Quaternion.Euler(0, m_imageRotationY, 0);
        var rotatedVector = rotation * transform.forward;
        return rotatedVector;
    }

    // TEST ONLY
    public void __M_Reset()
    {
        Debug.Log("Directional @Reset");
        CurrentPlayer = null;
    }

    private void ButtonPrompts()
    {
        var colliders = Physics.OverlapSphere(transform.position, m_interactionRadius, m_playerLayer);
        HashSet<PlayerController> currentPlayers = new HashSet<PlayerController>();

        foreach (var agent in colliders)
        {
            PlayerController playerController = agent.gameObject.GetComponent<PlayerController>();
            if (playerController == null) continue;

            currentPlayers.Add(playerController);

            if (playerController.playerNum == 1 && !playerController.isPerforming && !pOneButton.activeSelf)
            {
                pOneButton.SetActive(true);
                uiControlsOne.SetActive(false);
            }
            else if (playerController.playerNum == 2 && !playerController.isPerforming && !pTwoButton.activeSelf)
            {
                pTwoButton.SetActive(true);
                uiControlsTwo.SetActive(false);
            }

            if (playerController.playerNum == 1 && playerController.isPerforming && pOneButton.activeSelf)
            {
                pOneButton.SetActive(false);
                uiControlsOne.SetActive(true);
            }
            else if (playerController.playerNum == 2 && playerController.isPerforming && pTwoButton.activeSelf)
            {
                pTwoButton.SetActive(false);
                uiControlsTwo.SetActive(true);
            }

            RotateToCamera(playerController.playerFreeLook, playerController);
        }

        foreach (var previousPlayer in previousPlayers)
        {
            if (!currentPlayers.Contains(previousPlayer))
            {
                if (previousPlayer.playerNum == 1 && pOneButton.activeSelf)
                {
                    pOneButton.SetActive(false);
                    uiControlsOne.SetActive(false);
                }
                else if (previousPlayer.playerNum == 2 && pTwoButton.activeSelf)
                {
                    pTwoButton.SetActive(false);
                    uiControlsTwo.SetActive(false);
                }
            }
        }

        previousPlayers = currentPlayers;
    }

    private void RotateToCamera(CinemachineFreeLook playerCam, PlayerController playerController)
    {
        if (pOneButton.activeSelf && playerController.playerNum == 1)
        {
            Vector3 directionToCamera = playerCam.transform.position - pOneButton.transform.position;
            directionToCamera.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            pOneButton.transform.rotation =
                Quaternion.Slerp(pOneButton.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else if (pTwoButton.activeSelf && playerController.playerNum == 2)
        {
            Vector3 directionToCamera = playerCam.transform.position - pTwoButton.transform.position;
            directionToCamera.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            pTwoButton.transform.rotation =
                Quaternion.Slerp(pTwoButton.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    #endregion
}