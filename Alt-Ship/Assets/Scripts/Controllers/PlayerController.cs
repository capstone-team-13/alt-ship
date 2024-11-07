using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;

public class PlayerController : Controller<PlayerModel>
{
    [Header("Concrete Reference")] [SerializeField]
    private Rigidbody m_rigidBody;

    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;
    private InputAction look;

    public Material playerOneMat;
    public Material playerTwoMat;
    public MeshRenderer meshRenderer;

    private Vector2 moveDir;

    public Camera cinemachineCam;


    private void Awake()
    {
        PlayerCounter.playerCount += 1;
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");

        if (PlayerCounter.playerCount == 1)
        {
            meshRenderer.material = playerOneMat;
        }
        else if (PlayerCounter.playerCount == 2)
        {
            meshRenderer.material = playerTwoMat;
        }
    }

    #region Unity Callbacks

    [UsedImplicitly]
    private void Update()
    {
        if (Model.Dead) return;

        if (player.enabled)
        {
            MovePlayer();
        }
    }

    #endregion

    private void OnEnable()
    {
        look = player.FindAction("Camera");
        move = player.FindAction("Movement");
        player.Enable();
    }

    private void OnDisable()
    {
        player.Disable();
    }

    void MovePlayer()
    {
        Vector2 movementInput = PlayerCtrlInput();

        Vector3 camForward = cinemachineCam.transform.forward;
        Vector3 camRight = cinemachineCam.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * movementInput.y + camRight * movementInput.x).normalized;

        // TODO: Refactor to new input system
        var input = moveDir;
        __M_UpdateDirection(input);

        transform.Translate(Model.Velocity * Time.deltaTime);
    }

    private Vector2 PlayerCtrlInput()
    {
        Vector2 moveV = Vector2.zero;

        moveV = new Vector2(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y);

        return moveV;
    }


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