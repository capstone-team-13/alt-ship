using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTentacleCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tentacleLeft;
    [SerializeField] private GameObject tentacleRight;
    [SerializeField] private SailMovementSystem sailMovementSystem;
    [SerializeField] private SailFunction sailFunction;
    [SerializeField] private Animator leftAnimator;
    [SerializeField] private Animator rightAnimator;

    [Header("Variables")]
    [SerializeField] private float timeToAttack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitiateAttack(bool left, bool right)
    {
        if(left && right)
        {
            tentacleLeft.SetActive(true);
            tentacleRight.SetActive(true);
        }
        else if(left && !right)
        {
            tentacleLeft.SetActive(true);
        }
        else if (right && !left)
        {
            tentacleRight.SetActive(true);
        }
    }

}
