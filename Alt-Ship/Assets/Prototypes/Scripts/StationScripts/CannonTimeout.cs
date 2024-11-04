using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTimeout : MonoBehaviour
{
    private float timeLeft = 4f;
    private bool isEnabled = false;
    // Start is called before the first frame update

    private void OnEnable()
    {
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft < 0)
            {
                Destroy(this.transform.gameObject);
            }
        }
    }
}
