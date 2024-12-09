using EE.AMVCC;
using UnityEngine;

public class CannonController : Controller<CannonModel>
{
    public Rigidbody cannonBall;
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
  //      Model.Projectile = projectile;
    }

    private void __M_Shoot()
    {
        Debug.Log("Shoot Cannon!");
        var projectile = Model.Projectile;
        // TODO: REMOVE THIS! 
        {
            Rigidbody launchedBall = Instantiate(cannonBall, Model.ShootAt.position, Quaternion.identity);
            launchedBall.AddForce(Model.ShootAt.forward * Model.InitialForce, ForceMode.Impulse);
        }
    }

    #endregion
}