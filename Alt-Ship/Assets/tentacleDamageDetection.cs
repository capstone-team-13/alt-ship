using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tentacleDamageDetection : MonoBehaviour
{
    public Animator animator;
    public bool toggle = false;
    public bool attackStart;

    public GameObject targetPosition;

    public GameObject getSheep = null;

    public float MaxTime = 20f;
    private float CountDown = 0f;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("CannonBall"))
        {
            if (collider.GetComponent<CannonTimeout>())
            {
                var name = collider.name;
                CannonTimeout thisBall = collider.GetComponent<CannonTimeout>();
                thisBall.isEnabled = false;
                if(collider.name == name && !toggle)
                {
                    toggle = true;
                    wasHit();
                }
            } 
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!toggle && collider.CompareTag("Sheep"))
        {

        }

    }

    void wasHit()
    {
        animator.SetTrigger("wasHit");
        Debug.Log("was hit");
    }

    void attackSheep()
    {

        animator.SetTrigger("attackSheep");
    }


}
