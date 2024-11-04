using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;
using Application = EE.AMVCC.Application;

public class PlayerController : Controller<PlayerModel>
{
    [Header("Concrete Reference")] [SerializeField]
    private Rigidbody m_rigidBody;

    #region Unity Callbacks

    [UsedImplicitly]
    private void Update()
    {
        if (Model.Dead) return;

        // TODO: Refactor to new input system
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        __M_UpdateDirection(input);

        transform.Translate(Model.Velocity * Time.deltaTime);
    }

    #endregion

    #region API

    public override void Notify<TCommand>(TCommand command)
    {
        if (command is not PlayerCommand) return;

        switch (command)
        {
            case PlayerCommand.Dead deadCommand:
                if (deadCommand.Player != Model) return;
                Model.Dead = true;
                break;
        }
    }

    #endregion

    #region Internal

    private void __M_UpdateDirection(Vector3 input)
    {
        if (Model.Direction == input) return;

        Model.Direction = input;
        Application.Instance.Push(new PlayerCommand.Move(Model, input));
    }

    #endregion
}