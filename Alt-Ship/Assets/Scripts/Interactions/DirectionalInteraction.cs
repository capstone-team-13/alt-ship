using EE.Interactions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalInteraction : Interactable
{
    #region Editor API

    [Header("Directional Interaction")]
    [SerializeField]
    private float m_interactionRadius = 2.0f;

    [SerializeField][Range(-180, 180)] private float m_imageRotationY = 0;

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

    protected override bool CanInteract()
    {
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

        Debug.Log("Button Prompts Called");

        foreach ( var agent in colliders)
        {
            Debug.Log("Collider Found: " + agent.gameObject.name);

            PlayerController playerController = agent.gameObject.GetComponent<PlayerController>();
            if(playerController == null) continue;

            Debug.Log("Passed Null Check");
            Debug.Log("Current Performer: " + CurrentPerformer);
            Debug.Log("Player Num: " + playerController.playerNum);
            currentPlayers.Add(playerController);

                if(playerController.playerNum == 1 && !playerController.isPerforming && !pOneButton.activeSelf)
                {

                Debug.Log("Player confirmed not performer");

                pOneButton.SetActive(true);
                uiControlsOne.SetActive(false);
                }
                else if (playerController.playerNum == 2 && !playerController.isPerforming && !pTwoButton.activeSelf)
                {
                pTwoButton.SetActive(true);
                uiControlsTwo.SetActive(false);
                }

                if(playerController.playerNum == 1 && playerController.isPerforming && pOneButton.activeSelf)
                {

                Debug.Log("Player confirmed is performer");

                pOneButton.SetActive(false);
                uiControlsOne.SetActive(true);
                }
                else if(playerController.playerNum == 2 && playerController.isPerforming && pTwoButton.activeSelf)
                {
                    pTwoButton.SetActive(false);
                uiControlsTwo.SetActive(true);
                }
        }

        foreach (var previousPlayer in previousPlayers)
        {
            if (!currentPlayers.Contains(previousPlayer))
            {
                if(previousPlayer.playerNum == 1 && pOneButton.activeSelf)
                {

                    Debug.Log("Player left collider");

                    pOneButton.SetActive(false);
                    uiControlsOne.SetActive(false);
                }
                else if(previousPlayer.playerNum == 2 && pTwoButton.activeSelf)
                {
                    pTwoButton.SetActive(false);
                    uiControlsTwo.SetActive(false);
                }
            }
        }

        previousPlayers = currentPlayers;
    }

    #endregion
}