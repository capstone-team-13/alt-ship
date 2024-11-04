using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CannonFire : MonoBehaviour
{
    public float force;
    public Rigidbody cannonBallPref;
    public Transform startPosition;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        launchBall();
    }

    void launchBall()
    {
        Rigidbody launchedBall = Instantiate(cannonBallPref, startPosition.position, Quaternion.identity);
        launchedBall.AddForce(startPosition.forward * force, ForceMode.Impulse);
    }
}
