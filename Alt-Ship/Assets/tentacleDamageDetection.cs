using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tentacleDamageDetection : MonoBehaviour
{

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("CannonBall"))
        {
            if (collider.GetComponent<CannonTimeout>())
            {
                CannonTimeout thisBall = collider.GetComponent<CannonTimeout>();
                thisBall.isEnabled = false;
            }
            wasHit();

        }
    }

    void wasHit()
    {
        Debug.Log("was hit");
    }


}
