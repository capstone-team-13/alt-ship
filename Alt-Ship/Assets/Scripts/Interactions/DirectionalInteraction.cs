using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;

public class DirectionalInteraction : Interactable
{
    #region Editor API

    [Header("Directional Interaction")]
    [SerializeField]
    private float m_interactionRadius = 2.0f;

    [SerializeField][Range(-180, 180)] private float m_imageRotationY = 0;

    [SerializeField] private LayerMask m_playerLayer;

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

            if (!behindObject)
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

    #endregion
}