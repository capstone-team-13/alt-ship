using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;
using Cinemachine;

public class PlayerController : Controller<PlayerModel>
{
    #region Editor API

    [Header("Concrete Reference")] [SerializeField]
    private Rigidbody m_rigidBody;

    [SerializeField] private Transform parentRotation;

    [SerializeField] private Camera m_camera;

    [SerializeField] private EE.Prototype.PC.PlayerController[] m_animator;

    // TODO: Remove
    public Material playerOneMat;
    public Material playerTwoMat;
    public MeshRenderer meshRenderer;

    public int playerNum;

    [SerializeField] private CinemachineFreeLook playerFreeLook;

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

    public void SetTargetPositionXZ(Vector3 position)
    {
        m_animator[playerNum - 1].SetTargetPositionXZ(position);
    }

    public void UpdatePlayerId()
    {
        playerNum = (int)++m_playerCount;
    }

    public void SetUpPlayerModel(int id)
    {
        if (id < 0 || id >= m_animator.Length)
        {
            Debug.LogError(
                $"Index out of range: playerNum - 1 = {id}, m_animator.Length = {m_animator.Length}");
            return;
        }

        var character = m_animator[id].gameObject;

        m_animator[id].gameObject.SetActive(true);

        var anotherAnimator = playerNum % m_animator.Length;
        if (anotherAnimator < 0 || anotherAnimator >= m_animator.Length)
        {
            Debug.LogError(
                $"Index out of range: anotherAnimator = {anotherAnimator}, m_animator.Length = {m_animator.Length}");
            return;
        }

        m_animator[anotherAnimator].gameObject.SetActive(false);

        var body = character.transform.Find("Body")?.gameObject;
        if (body == null)
        {
            Debug.LogWarning($"Body not found for character: {character.name}");
            return;
        }

        playerFreeLook.LookAt = body.transform;
        playerFreeLook.Follow = body.transform;

        playerNum = id + 1;
    }

    #endregion

    #region Unity Callbacks

    [UsedImplicitly]
    private void Awake()
    {
        if (playerNum == 1)
        {
            this.gameObject.name = "Player: " + playerNum;
            playerFreeLook.gameObject.layer = 11;
            m_camera.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI",
                "Detectable", "Goal", "Player1");
            m_camera.gameObject.layer = 11;
        }
        else if (playerNum == 2)
        {
            this.gameObject.name = "Player: " + playerNum;
            m_camera.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI",
                "Detectable", "Goal", "Player2");
            playerFreeLook.gameObject.layer = 12;
            m_camera.gameObject.layer = 12;
        }

        m_player = GetComponent<PlayerInput>().actions.FindActionMap("PlayerMovement");

        meshRenderer.material = m_playerCount switch
        {
            1 => playerOneMat,
            2 => playerTwoMat,
            _ => meshRenderer.material
        };

        if (!parentRotation)
        {
            parentRotation = GameObject.FindWithTag("Ship").transform;
        }
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
        m_animator[playerNum - 1].UserInput = movementInput;

        Vector3 direction = m_camera.transform.TransformDirection(movementInput);
        direction.y = 0;

        Quaternion inverseParentRotation = Quaternion.Inverse(parentRotation.rotation);
        direction = inverseParentRotation * direction;

        __M_UpdateDirection(direction);

        // Remove if players rotation matters
        // transform.rotation = parentRotation.rotation;

        transform.Translate(Model.Velocity * Time.deltaTime);
        m_animator[playerNum - 1].SetTargetPositionXZ(transform.position);
    }

    private void __M_UpdateDirection(Vector3 input)
    {
        if (Model.Direction == input) return;

        Model.Direction = input;
        Application.Instance.Push(new PlayerCommand.Move(Model, input));
    }

    #endregion
}