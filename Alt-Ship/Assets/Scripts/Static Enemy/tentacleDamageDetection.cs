using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tentacleDamageDetection : MonoBehaviour
{
    public Animator animator;
    public bool toggle = false;

    public GameObject targetPosition;

    private GameObject currentSheep = null;

    private GameObject enemyModel;

    public Transform ship;

    private bool isGrabbed;

    private Vector3 oldPosition;

    public Collider sheepDetection;
    public Collider hitboxDetection;


    private void Awake()
    {
        oldPosition = new Vector3(0f, 0f, 0f);
        enemyModel = this.gameObject;
    }

    private void Update()
    {
        // Rotates to correct position
        RotateTo();
    }

    void OnTriggerEnter(Collider collider)
    {
        // Determines if the collider was hit by a cannon ball
        if (collider.CompareTag("CannonBall") && this.GetComponent<Collider>() == hitboxDetection)
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
            return;
        }

        // Determines if a sheep is in range
        if (collider.CompareTag("Sheep") && this.GetComponent<Collider>() == sheepDetection && currentSheep == null && !toggle)
        {
            Debug.Log("triggered");
            currentSheep = collider.gameObject;
            StartCoroutine(attackSheepTime());
            
        }
    }

    // Determines if a sheep escaped range
    private void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject == currentSheep && this.GetComponent<Collider>() == sheepDetection)
        {
            Debug.Log("Left area");
            currentSheep = null;
        }
    }

    // Gives the player a safty period before being attacked
    private IEnumerator attackSheepTime()
    {
        yield return new WaitForSeconds(5f);

        if (currentSheep != null && !toggle) {
            attackSheep();
        }
            
    }

    // Animation to get hit by a cannon ball
    void wasHit()
    {
        animator.SetTrigger("wasHit");
        Debug.Log("was hit");
    }

    // Animation to attack a sheep
    void attackSheep()
    {
        animator.SetTrigger("attackSheep");
    }

    // Function to be played when sheep is taken
    public void takeSheep()
    {
        Debug.Log("Take sheep initiated");
        if(currentSheep != null)
            {
            currentSheep.transform.SetParent(targetPosition.transform);
            currentSheep.transform.localPosition = new Vector3(0f,0f,0f);
            isGrabbed = true;
            Debug.Log("Sheep taken");
            }

    }

    // Function to orient the tentacle to the proper location
    void RotateTo()
    {
        if (currentSheep == null)
        {
            enemyModel.transform.LookAt(ship.position);
            enemyModel.transform.eulerAngles = new Vector3(0f, enemyModel.transform.eulerAngles.y - 90f, 0f);
        }
        else if (isGrabbed)
        {
            enemyModel.transform.LookAt(oldPosition);
            enemyModel.transform.eulerAngles = new Vector3(0f, enemyModel.transform.eulerAngles.y - 90f, 0f);
        }
        else if(currentSheep != null)
        {
            oldPosition = currentSheep.transform.position;
            enemyModel.transform.LookAt(currentSheep.transform.position);
            enemyModel.transform.eulerAngles = new Vector3(0f, enemyModel.transform.eulerAngles.y - 90f, 0f);
        }
    }

}
