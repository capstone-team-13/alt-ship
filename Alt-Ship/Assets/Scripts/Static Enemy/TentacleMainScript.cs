using System.Collections;
using UnityEngine;

public class TentacleDamageDetection : MonoBehaviour
{
    public Animator animator;
    public bool toggle = false;

    public GameObject targetPosition;
    public GameObject currentSheep = null;

    public Transform ship;

    private bool isGrabbed = false;
    private Vector3 oldPosition;

    private void Awake()
    {
        oldPosition = Vector3.zero;
    }

    private void Update()
    {
        RotateTo();
    }

    public IEnumerator AttackSheepTime()
    {
        yield return new WaitForSeconds(5f);

        if (currentSheep != null && !toggle)
        {
            animator.SetTrigger("attackSheep");
        }
    }

    public void HandleCannonBallHit(Collider cannonBall)
    {
        if (cannonBall.GetComponent<CannonTimeout>())
        {
            var name = cannonBall.name;
            CannonTimeout thisBall = cannonBall.GetComponent<CannonTimeout>();
            thisBall.isEnabled = false;

            if (cannonBall.name == name && !toggle)
            {
                toggle = true;
                animator.SetTrigger("wasHit");
                Debug.Log("Hit by cannonball!");
            }
        }
    }

    public void attackTentacle()
    {
        Debug.Log("Take sheep initiated");

        if (currentSheep != null)
        {
            currentSheep.transform.SetParent(targetPosition.transform);
            currentSheep.transform.localPosition = Vector3.zero;
            isGrabbed = true;
            Debug.Log("Sheep taken!");
        }
    }

    private void RotateTo()
    {
        Transform tentacleTransform = transform;

        if (currentSheep == null)
        {
            tentacleTransform.LookAt(ship.position);
            tentacleTransform.eulerAngles = new Vector3(0f, tentacleTransform.eulerAngles.y - 90f, 0f);
        }
        else if (isGrabbed)
        {
            tentacleTransform.LookAt(oldPosition);
            tentacleTransform.eulerAngles = new Vector3(0f, tentacleTransform.eulerAngles.y - 90f, 0f);
        }
        else if (currentSheep != null)
        {
            oldPosition = currentSheep.transform.position;
            tentacleTransform.LookAt(currentSheep.transform.position);
            tentacleTransform.eulerAngles = new Vector3(0f, tentacleTransform.eulerAngles.y - 90f, 0f);
        }
    }
}