using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tentacleDamageDetection : MonoBehaviour
{
    public Animator animator;
    public bool toggle = false;

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

    void wasHit()
    {
        animator.SetTrigger("wasHit");
        Debug.Log("was hit");
    }


}
