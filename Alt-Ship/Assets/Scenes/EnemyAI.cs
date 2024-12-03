using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform sheepTarget; // Assign the sheep GameObject or parent GameObject
    public float moveSpeed = 3f;

    void Start()
    {
        GameObject sheep = GameObject.FindWithTag("Sheep"); // Tag the sheep with "Sheep"
        if (sheep != null)
        {
            sheepTarget = sheep.transform;
        }
    }


    void Update()
    {
        if (sheepTarget != null)
        {
            // Move towards the sheep
            Vector3 direction = (sheepTarget.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Optional: Face the target
            transform.LookAt(sheepTarget);
        }
    }
}
