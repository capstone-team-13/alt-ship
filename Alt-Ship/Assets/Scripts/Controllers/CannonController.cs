using EE.AMVCC;
using EE.Interactions;
using JetBrains.Annotations;
using UnityEngine;

public class CannonController : Controller<CannonModel>
{
    [Header("Concrete Reference")] [SerializeField]
    private Interactable m_interactable;

    #region Unity Callbacks

    [UsedImplicitly]
    private void Awake()
    {
        m_interactable.CanInteract = __M_CanInteract;
    }

    [UsedImplicitly]
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Model.InteractionRadius);
    }

    #endregion

    #region API

    public override void Notify<TCommand>(TCommand command)
    {
    }

    public void Interact()
    {
        // TODO: Load Logic
        __M_Shoot();
    }

    #endregion

    #region Internal

    private void __M_Load(GameObject projectile)
    {
        // Valid GameObject Type

        Model.Projectile = projectile;
    }

    private void __M_Shoot()
    {
        Debug.Log("Shoot Cannon!");
    }

    private bool __M_CanInteract()
    {
        var colliders = Physics.OverlapSphere(transform.position, Model.InteractionRadius, Model.PlayerLayer);
        // TODO: Add condition that player behind the cannon
        return colliders.Length > 0;
    }

    #endregion
}