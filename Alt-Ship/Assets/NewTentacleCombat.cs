using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTentacleCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tentacleLeft;
    [SerializeField] private GameObject tentacleRight;

    [SerializeField] private GameObject tentacleGrabLeft;
    [SerializeField] private GameObject tentacleGrabRight;

    [SerializeField] private Animator grabAnimatorLeft;
    [SerializeField] private Animator grabAnimatorRight;

    [SerializeField] private NewTentacleBehaviour tentacleBehaviourLeft;
    [SerializeField] private NewTentacleBehaviour tentacleBehaviourRight;

    [SerializeField] private SailMovementSystem sailMovementSystem;
    [SerializeField] private SailFunction sailFunction;

    [Header("Variables")]
    [SerializeField] private GameObject targetLeft;
    [SerializeField] private GameObject targetRight;
    public GameObject[] sheepArray;

    private bool sameTarget;
    private int pickTarget;

    private void Update()
    {
        if(tentacleBehaviourLeft.isStarted || tentacleBehaviourRight.isStarted)
        {
            if(tentacleBehaviourLeft.isFinished && tentacleBehaviourRight.isFinished)
            {
                tentacleBehaviourLeft.ResetVariables();
                tentacleBehaviourRight.ResetVariables();
                StartCoroutine(GrabTentacleDisable(grabAnimatorRight));
                StartCoroutine(GrabTentacleDisable(grabAnimatorLeft));
                tentacleLeft.SetActive(false);
                tentacleRight.SetActive(false);
                targetLeft = null;
                targetRight = null;



                StartShip();
            }
        }
    }

    public void InitiateMovement(bool left, bool right)
    {
        tentacleGrabLeft.SetActive(true);
        tentacleGrabRight.SetActive(true);

        if(left && right)
        {
            return;
        }
        else if (left)
        {
            StartCoroutine(GrabTentacleFall(grabAnimatorRight));
        }
        else if (right) 
        {
            StartCoroutine(GrabTentacleFall(grabAnimatorLeft));
        }
    }

    private IEnumerator GrabTentacleFall(Animator animator)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 2f);

        StartCoroutine(GrabTentacleDisable(animator));
    }

    private IEnumerator GrabTentacleDisable(Animator animator)
    {
        animator.SetTrigger("Retreat");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 2f);

        animator.gameObject.SetActive(false);
    }

    public void InitiateAttack(bool left, bool right)
    {
        if(left && right)
        {
            tentacleLeft.SetActive(true);
            tentacleRight.SetActive(true);

            PickTargets(ref targetLeft, targetRight);
            PickTargets(ref targetRight, targetLeft);

            tentacleBehaviourLeft.setTarget(targetLeft);
            tentacleBehaviourRight.setTarget(targetRight);

            if (sameTarget)
            {
                if (pickTarget == 1)
                {
                    tentacleBehaviourLeft.notFirst = true;
                    tentacleBehaviourRight.notFirst = false;
                }
                else
                {
                    tentacleBehaviourLeft.notFirst = false;
                    tentacleBehaviourRight.notFirst = true;
                }
            }
        }
        else if(left && !right)
        {
            tentacleLeft.SetActive(true);

            PickTargets(ref targetLeft, targetRight);

            tentacleBehaviourLeft.setTarget(targetLeft);
        }
        else if (right && !left)
        {
            tentacleRight.SetActive(true);

            PickTargets(ref targetRight, targetLeft);

            tentacleBehaviourRight.setTarget(targetRight);
        }
    }

    private void PickTargets(ref GameObject target, GameObject oppositeTarget)
    {
        List<GameObject> availableSheep = new List<GameObject>();
        foreach (GameObject sheep in sheepArray)
        {
            if (sheep.activeSelf)
            {
                availableSheep.Add(sheep);
            }
        }

        if (availableSheep.Count == 0) return;

        if (availableSheep.Count == 1)
        {
            target = availableSheep[0];
            pickTentacle();
            return;
        }

        GameObject newTarget;

        do
        {
            newTarget = availableSheep[Random.Range(0, availableSheep.Count)];
        }
        while (newTarget == oppositeTarget);

        target = newTarget;
    }

    private void pickTentacle()
    {
        sameTarget = true;
        if(Random.value < .5f)
        {
            pickTarget = 1;
        }
        else 
        { 
            pickTarget = 2;
        }
    }

    private void StartShip()
    {
        sailMovementSystem.DisableSailing();
        sailFunction.disableSail();
    }

}
