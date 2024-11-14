using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;

public class RangeInteractable : Interactable
{
    #region Editor API

    [Header("Range Interaction")]
    [SerializeField]
    private float m_interactionRadius = 2.0f;

    [SerializeField] private LayerMask m_playerLayer;

    #endregion

    #region Unity Callbacks

    [UsedImplicitly]
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_interactionRadius);
    }

    #endregion

    #region API

    protected override bool CanInteract()
    {
        var notInCoolingDown = base.CanInteract();
        var colliders = Physics.OverlapSphere(transform.position, m_interactionRadius, m_playerLayer);
        return notInCoolingDown && colliders.Length > 0;
    }
    protected override bool PlayerExit()
    {
        var distanceToPlayer = (CurrentPlayer.transform.position - transform.position).magnitude;
        return distanceToPlayer > m_interactionRadius;
    }

    #endregion
}