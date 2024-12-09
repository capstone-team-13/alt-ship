using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTimeout : MonoBehaviour
{
    [SerializeField] private GameObject particles;

    [SerializeField] private MeshRenderer meshRenderer;

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
        if (isEnabled)
        {
            prevPos = transform.position;
            timeLeft -= Time.deltaTime;
            if(timeLeft < 0)
            {
                selfDestruct();
            }
        }
        else if (!isEnabled)
        {
            this.transform.position = prevPos;
            meshRenderer.enabled = false;
        }

        if(!isEnabled && !particles.activeSelf)
        {
            selfDestruct();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Cannon")
        {
            TriggerParticles();
            isEnabled = false;
        }
    }

    private void TriggerParticles()
    {
        particles.SetActive(true);
    }

    private void selfDestruct()
    {
        Destroy(this.transform.gameObject);
    }

}
