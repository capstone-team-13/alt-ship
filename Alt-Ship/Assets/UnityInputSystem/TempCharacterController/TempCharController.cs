using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempCharController : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;

    private Rigidbody playerRB;

    private Vector3 movementTranslator;

    public Material playerOneMat;
    public Material playerTwoMat;

    [SerializeField]
    private float playerSpeed;

    private void Awake()
    {
        PlayerCounter.playerCount += 1;
        playerRB = this.GetComponent<Rigidbody>();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        
        if(PlayerCounter.playerCount == 1)
        {
            this.GetComponent<MeshRenderer>().material = playerOneMat;
        }
        else if(PlayerCounter.playerCount == 2)
        {
            this.GetComponent<MeshRenderer>().material = playerTwoMat;
        }
    }

    private void Update()
    {
        MovePlayer();
    }

    private void OnEnable()
    {
        move = player.FindAction("Movement");
        player.Enable();
    }

    private void OnDisable()
    {
        player.Disable();
    }

    void MovePlayer()
    {
        Debug.Log(move.ReadValue<Vector2>());
        movementTranslator = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y);
        playerRB.AddForce((movementTranslator * playerSpeed), ForceMode.Force);
    }


}
