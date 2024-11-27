using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;

public class ShipController : Controller<ShipModel>
{
    [Header("Concrete Reference")] [SerializeField]
    private Rigidbody m_rigidBody;

    #region Unity Callbacks

    [UsedImplicitly]
    private void Update()
    {
        Model.SailDirection = transform.forward;

        Quaternion currentRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(Model.TargetEulerAngles);
        transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, Model.LerpSpeed * Time.deltaTime);
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
        // TODO: Add Current Velocity
        //var newVelocity = Model.Speed * Model.SailDirection;
        //m_rigidBody.velocity = newVelocity;

        shipMovement();

    }

    #endregion

    #region API

    public override void Notify<TCommand>(TCommand command)
    {
        if (command is not ShipCommand) return;

        switch (command)
        {
            case ShipCommand.Steer steerCommand:
                var sign = steerCommand.RotationSign;
                __M_Steer(sign);
                break;
        }
    }

    #endregion

    #region Internal

    private void __M_Steer(float sign)
    {
        var rotationDelta = sign * Model.RotationSpeed * Time.deltaTime;
        Model.TargetEulerAngles.y += rotationDelta;
    }

    private void shipMovement()
    {
        Debug.Log(m_rigidBody.velocity);
        m_rigidBody.AddForce(Model.Speed * Model.SailDirection, ForceMode.Force);
        var newVelocity = Model.Speed * Model.SailDirection;
        m_rigidBody.velocity = newVelocity;
    }

    #endregion
}