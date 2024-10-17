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
        if (move.IsPressed())
        {
            if (move.activeControl.device.name == "Keyboard" || move.activeControl.device.name == "Mouse")
            {
                //Debug.Log(move.ReadValue<Vector2>());
                movementTranslator = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y);
                playerRB.AddRelativeForce((movementTranslator * playerSpeed), ForceMode.Force);
            }
        }
        else
        {
            //Debug.Log(move.ReadValue<Vector3>());
            
            if(move.ReadValue<Vector3>().x != 0 || move.ReadValue<Vector3>().z != 0)
            {
                Vector3 cameraRotation = cinemachineCam.transform.eulerAngles;
                var playerRotation = playerRB.transform.eulerAngles;
                var newRotation = cameraRotation;
                newRotation.x = playerRotation.x;
                playerRB.transform.rotation = Quaternion.Euler(newRotation);
            }
            playerRB.AddForce(new Vector3((cinemachineCam.transform.forward.x * move.ReadValue<Vector3>().x) * playerSpeed, (cinemachineCam.transform.forward.y * move.ReadValue<Vector3>().y) * playerSpeed, (cinemachineCam.transform.forward.z * move.ReadValue<Vector3>().z) * playerSpeed), ForceMode.Force);
            Debug.Log("X:" + cinemachineCam.transform.forward.x * move.ReadValue<Vector3>().x);
            Debug.Log("Y:" + cinemachineCam.transform.forward.y * move.ReadValue<Vector3>().y);
            Debug.Log("Z:" + cinemachineCam.transform.forward.z * move.ReadValue<Vector3>().z);
        }

    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(playerRB.transform.position, playerRB.transform.forward);

    }

}

