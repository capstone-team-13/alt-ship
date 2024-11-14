using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;

public class PlayerController : Controller<PlayerModel>
{
    #region Editor API

    [Header("Concrete Reference")] [SerializeField]
    private Rigidbody m_rigidBody;

    [SerializeField] private Camera m_camera;

    // TODO: Remove
    public Material playerOneMat;
    public Material playerTwoMat;
    public MeshRenderer meshRenderer;

    #endregion

    #region API

    public override void Notify<TCommand>(TCommand command)
    {
        if (command is not PlayerCommand playerCommand || playerCommand.Player != Model) return;

        switch (command)
        {
            case PlayerCommand.Dead:
                Model.Dead = true;
                break;
            case PlayerCommand.ChangeSpeed changeSpeedCommand:
                Model.Speed = changeSpeedCommand.Speed;
                break;
        }
    }

    #endregion

    #region Unity Callbacks

    [UsedImplicitly]
    private void Awake()
    {
        ++m_playerCount;
        m_player = GetComponent<PlayerInput>().actions.FindActionMap("PlayerMovement");

        meshRenderer.material = m_playerCount switch
        {
            1 => playerOneMat,
            2 => playerTwoMat,
            _ => meshRenderer.material
        };
    }

    [UsedImplicitly]
    private void Update()
    {
        if (Model.Dead) return;
        if (m_player.enabled) __M_Move();
    }

    [UsedImplicitly]
    private void OnEnable()
    {
        m_lookAction = m_player.FindAction("Camera");
        m_moveAction = m_player.FindAction("Movement");
        m_player.Enable();
    }

    [UsedImplicitly]
    private void OnDisable()
    {
        m_player.Disable();
    }

    #endregion

    #region Internal

    private InputActionMap m_player;
    private InputAction m_moveAction;
    private InputAction m_lookAction;

    private static uint m_playerCount;

    private Vector3 __M_GetPlayerInput()
    {
        var value = m_moveAction.ReadValue<Vector2>();
        var input = new Vector3(value.x, 0, value.y);
        return input;
    }

    private void __M_Move()
    {
        Vector3 movementInput = __M_GetPlayerInput();

        Vector3 direction = m_camera.transform.TransformDirection(movementInput);
        direction.y = 0;

        __M_UpdateDirection(direction);

        transform.Translate(Model.Velocity * Time.deltaTime);
    }

    private void __M_UpdateDirection(Vector3 input)
    {
        if (Model.Direction == input) return;

        Model.Direction = input;
        Application.Instance.Push(new PlayerCommand.Move(Model, input));
    }

    #endregion
}