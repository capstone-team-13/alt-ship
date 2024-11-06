using EE.AMVCC;
using UnityEngine;

public class CannonController : Controller<CannonModel>
{

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

    #endregion
}