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

    private void __M_Load(Rigidbody projectile)
    {
        // Valid GameObject Type
        Model.Projectile = projectile;
    }

    private void __M_Shoot()
    {
        Debug.Log("Shoot Cannon!");
        var projectile = Model.Projectile;
        // TODO: REMOVE THIS! 
        {
            projectile.gameObject.SetActive(true);
            projectile.position = Model.ShootAt.position;
            projectile.velocity = Vector3.zero;
            projectile.angularVelocity = Vector3.zero;
        }
        projectile.AddForce(Model.ShootAt.forward * Model.InitialForce, ForceMode.Impulse);
    }

    #endregion
}