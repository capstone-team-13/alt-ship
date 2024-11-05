using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonFire : MonoBehaviour
{
    [SerializeField]
    private InputAction fireCannon;

    public float force;
    public Rigidbody cannonBallPref;
    public Transform startPosition;

    public float cooldown = 2f;
    public float timestamp;

    public Material cantFire;
    public Material canFire;

    private bool toggle1;

    void OnEnable()
    {
        timestamp = 0;
        fireCannon.Enable();
        fireCannon.performed += launchBall;
        toggle1 = false;
    }

    private void OnDisable()
    {
        fireCannon.performed -= launchBall;
        fireCannon.Disable();
    }

    private void Update()
    {
        if(timestamp <= Time.time && toggle1 == false)
        {
            this.GetComponent<MeshRenderer>().material = canFire;
            toggle1 = true;
        }
    }

    void launchBall(InputAction.CallbackContext context)
    {
        if (timestamp <= Time.time)
        {
            Rigidbody launchedBall = Instantiate(cannonBallPref, startPosition.position, Quaternion.identity);
            launchedBall.AddForce(startPosition.forward * force, ForceMode.Impulse);
            this.GetComponent<MeshRenderer>().material = cantFire;
            toggle1 = false;
            timestamp = Time.time + cooldown;
        }
    }
}
