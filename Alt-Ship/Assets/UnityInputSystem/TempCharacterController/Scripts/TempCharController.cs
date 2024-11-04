using UnityEngine;
using UnityEngine.InputSystem;

public class TempCharController : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;
    private InputAction look;

    private Rigidbody playerRB;

    private Vector3 movementTranslator;

    public Material playerOneMat;
    public Material playerTwoMat;

    public Camera cinemachineCam;

    public float camSpeed = 2;



    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float playerMaxSpeed;
    [SerializeField]
    private float playerDrag;

    private void Awake()
    {

        PlayerCounter.playerCount += 1;
        playerRB = this.GetComponent<Rigidbody>();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");

        if (PlayerCounter.playerCount == 1)
        {
            this.GetComponent<MeshRenderer>().material = playerOneMat;
        }
        else if (PlayerCounter.playerCount == 2)
        {
            this.GetComponent<MeshRenderer>().material = playerTwoMat;
        }

        playerRB.drag = playerDrag;
    }

    private void Update()
    {
        if (player.enabled)
        {
            MovePlayer();
        }

    }

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

        if(playerRB.velocity.magnitude < playerMaxSpeed)
        {
            playerRB.AddForce(moveDir * playerSpeed, ForceMode.Force);
        }

    }

    private Vector2 PlayerCtrlInput()
    {
        Vector2 moveV = Vector2.zero;

        if(Keyboard.current != null)
        {
          //  move.ReadValue<Vector2>();
        }
        if(Gamepad.current != null)
        {
            moveV = new Vector2(move.ReadValue<Vector3>().x, move.ReadValue<Vector3>().y);
        }
        return moveV;
    }


}

