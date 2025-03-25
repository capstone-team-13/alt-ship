using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Application = EE.AMVCC.Application;



public class NewTentacleBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NewTentacleBehaviour other;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject tentTip;
    [SerializeField] private float timeToAttack;

    public bool isFinished = true;
    public bool isStarted = false;
    public bool notFirst = false;

    private bool timerReset = false;
    private float timer = 0f;
    public bool sheepIsGrabbed = false;
    private bool wasAttacked = false;
    private bool finishedWaiting = false;
    private bool toggle = false;

    private void OnEnable()
    {
        isFinished = false;
        isStarted = true;
    }

    private void Update()
    {
        if (target != null && !sheepIsGrabbed && !notFirst)
        {
            LookAtTarget();
        }

        if (!notFirst && !finishedWaiting)
        {
            WaitingToStrike();
        }
    }

    public void setTarget(GameObject sheep)
    {
        target = sheep;
    }

    private void LookAtTarget()
    {
        Vector3 direction = (target.transform.position - this.transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, Time.deltaTime * 3f);
    }

    private void WaitingToStrike()
    {
        if (!timerReset)
        {
            timerReset = true;
            timer = 0f;
        }

        timer += Time.deltaTime;

        if (timer >= timeToAttack && !wasAttacked)
        {
            finishedWaiting = true;
            StartCoroutine(TentacleAttacking());
        }

    }

    private void GrabSheep()
    {
        if (!wasAttacked)
        {
            Debug.Log("Sheep Grabbed");

            target.transform.SetParent(tentTip.transform);
            target.transform.localPosition = Vector3.zero;
            sheepIsGrabbed = true;
        }


    }

    IEnumerator TentacleAttacking()
    {
        animator.SetTrigger("InitiateAttack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        StartCoroutine(TentacleRetreating());
    }

    IEnumerator TentacleRetreating()
    {
        if (!wasAttacked)
        {
            animator.SetTrigger("InitiateRetreat");
        }

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        if (sheepIsGrabbed)
        {
            Application.Instance.Push(new ShipCommand.Damage(1));
            target.SetActive(false);
            isFinished = true;
        }
        else if (wasAttacked)
        {
            isFinished = true;
        }


    }
    public void ResetVariables()
    {
        isStarted = false;
        notFirst = false;
        timerReset = false;
        timer = 0f;
        sheepIsGrabbed = false;
        wasAttacked = false;
        finishedWaiting = false;
        target = null;

        animator.ResetTrigger("InitiateAttack");
        animator.ResetTrigger("InitiateRetreat");
        animator.ResetTrigger("WasHit");
    }

    public void WasAttacked()
    {
        wasAttacked = true;
        animator.SetTrigger("WasHit");
        StartCoroutine(TentacleRetreating());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("CannonBall"))
        {
            HandleCannonBallHit(other);
        }
    }

    private void HandleCannonBallHit(Collider cannonBall)
    {
        Debug.Log("Check One");
        if (cannonBall.GetComponent<CannonTimeout>())
        {
            Debug.Log("Check Two");

            var name = cannonBall.name;
            CannonTimeout thisBall = cannonBall.GetComponent<CannonTimeout>();
            thisBall.isEnabled = false;

            if (cannonBall.name == name && !toggle)
            {
                Debug.Log("Check Three");

                toggle = true;
                WasAttacked();
            }
        }
    }

}
