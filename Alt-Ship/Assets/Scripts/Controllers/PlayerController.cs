using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Application = EE.AMVCC.Application;
using Cinemachine;
using EE.CU;

public class PlayerController : Controller<PlayerModel>
{
    #region Editor API

    [Header("Concrete Reference")] [SerializeField]
    private Rigidbody m_rigidBody;

    [SerializeField] private Transform parentRotation;

    [SerializeField] private Camera m_camera;

    [SerializeField] private EE.Prototype.PC.PlayerController[] m_controllers;

    // TODO: Remove
    public Material playerOneMat;
    public Material playerTwoMat;
    public MeshRenderer meshRenderer;

    public int playerNum;

    private Vector3 direction;

    public bool isPerforming = false;

    public CinemachineFreeLook playerFreeLook;

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

    public void SetUpPlayerModel(int id)
    {
        if (id < 0 || id >= m_controllers.Length)
        {
            Debug.LogError(
                $"Index out of range: playerNum - 1 = {id}, m_controllers.Length = {m_controllers.Length}");
            return;
        }

        var character = m_controllers[id];
        character.gameObject.SetActive(true);

        var anotherAnimator = playerNum % m_controllers.Length;
        if (anotherAnimator != id)
        {
            if (anotherAnimator < 0 || anotherAnimator >= m_controllers.Length)
            {
                Debug.LogError(
                    $"Index out of range: anotherAnimator = {anotherAnimator}, m_controllers.Length = {m_controllers.Length}");
                return;
            }

            m_controllers[anotherAnimator].gameObject.SetActive(false);
        }

        playerFreeLook.LookAt = character.Position;
        playerFreeLook.Follow = character.Position;

        playerNum = id + 1;
    }

    public void ResetTransform()
    {
        transform.rotation = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
    }

    #endregion

    #region Unity Callbacks

    [UsedImplicitly]
    private void Awake()
    {
        if (playerNum == 1)
        {
            gameObject.name = "Player: " + playerNum;
            playerFreeLook.gameObject.layer = 11;
            m_camera.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI",
                "Detectable", "Goal", "Player1");
            m_camera.gameObject.layer = 11;
        }
        else if (playerNum == 2)
        {
            gameObject.name = "Player: " + playerNum;
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
            GameObject go = GameObject.FindWithTag("ShipParent");
            if (go == null)
            {
                Debug.LogError("Error: ShipParent object not found in the scene!");
                return;
            }

            parentRotation = go.transform;
        }
    }

    [UsedImplicitly]

    void FixedUpdate()
    {
        if (Model.Dead) return;

        Vector3 movementInput = __M_GetPlayerInput();
        __M_Move();

        m_controllers[playerNum - 1].UserInput = Model.Speed > Mathf.Epsilon ? movementInput : Vector3.zero;
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

    [UsedImplicitly]
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Debug.DrawRay(transform.position + Vector3.up * 0.01f, transform.forward * 5.0f);

        Gizmos.color = Color.blue;

        Debug.DrawRay(transform.position + Vector3.up * 0.01f, parentRotation.transform.forward * 2.0f);
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
        transform.rotation = parentRotation.transform.rotation;

        Vector3 movementInput = __M_GetPlayerInput();
        m_controllers[playerNum - 1].UserInput = movementInput;

        Vector3 direction = m_camera.transform.TransformDirection(movementInput);


        Quaternion inverseParentRotation = Quaternion.Inverse(parentRotation.rotation);
        direction = inverseParentRotation * direction;
        direction.y = 0;
        direction = direction.normalized;

        __M_UpdateDirection(direction);

        if (Model.Speed > Mathf.Epsilon)
        {
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                m_controllers[playerNum - 1].Rotation.localRotation = Quaternion.Slerp(
                    m_controllers[playerNum - 1].Rotation.localRotation,
                    Quaternion.LookRotation(direction, Vector3.up),
                    Time.deltaTime * 5f
                );
            }
        }

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