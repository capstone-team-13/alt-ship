using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTentacleBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject target;
    [SerializeField] private float timeToAttack;

    public bool isFinished = false;
    public bool isStarted = false;

    private void OnEnable()
    {
        isStarted = true;
    }

    private void Update()
    {
        if(target != null)
        {
            lookAtTarget();
        }
    }


    public void setTarget(GameObject sheep)
    {
        target = sheep;
    }

    private void lookAtTarget()
    {
        Vector3 direction = (target.transform.position - this.transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, Time.deltaTime * 3f);
    }
}
