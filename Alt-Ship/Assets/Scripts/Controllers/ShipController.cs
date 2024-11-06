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

        var currentEulerAngles = transform.localEulerAngles;
        if (currentEulerAngles.y > 180) currentEulerAngles.y -= 360;

        var smoothEulerAngles =
            Vector3.Lerp(currentEulerAngles, Model.TargetEulerAngles, Model.LerpSpeed * Time.deltaTime);
        transform.localEulerAngles = smoothEulerAngles;
    }


    [UsedImplicitly]
    private void FixedUpdate()
    {
        // TODO: Add Current Velocity
        var newVelocity = Model.Speed * Model.SailDirection;
        m_rigidBody.velocity = newVelocity;
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

        // TODO: Angle Constraints
        const float angleConstraint = 60.0f;
        // Model.TargetEulerAngles.y = Mathf.Max(Model.TargetEulerAngles.y + rotationDelta, angleConstraint);

        Model.TargetEulerAngles.y += rotationDelta;
    }

    #endregion
}