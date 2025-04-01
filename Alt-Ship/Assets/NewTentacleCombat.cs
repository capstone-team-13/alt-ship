using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTentacleCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tentacleLeft;
    [SerializeField] private GameObject tentacleRight;

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
                tentacleLeft.SetActive(false);
                tentacleRight.SetActive(false);
                targetLeft = null;
                targetRight = null;



                StartShip();
            }
        }
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
