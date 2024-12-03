using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // The sheep the enemy will follow
    public float moveSpeed = 5f; // Speed of enemy movement
    public float rotationSpeed = 5f; // Speed of enemy rotation
    public float stoppingDistance = 1.5f; // Minimum distance to stop near the sheep

    private Rigidbody rb; // Rigidbody for the physics-based movement

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Calculate direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Prevent enemies from constantly spinning around
        if (Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
            // Move the enemy toward the target
            rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

            // Smoothly rotate toward the target
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
