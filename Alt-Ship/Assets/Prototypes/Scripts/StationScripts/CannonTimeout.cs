using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTimeout : MonoBehaviour
{
    private float timeLeft = 4f;
    private bool isEnabled = false;

    Vector3 prevPos;
    // Start is called before the first frame update

    private void OnEnable()
    {
        prevPos = transform.position;
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        prevPos = transform.position;

        if (isEnabled)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft < 0)
            {
                Destroy(this.transform.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Cannon")
        {
            isEnabled = false;
            Destroy(this.transform.gameObject);
        }
    }

}
