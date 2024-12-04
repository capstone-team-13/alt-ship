using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign your enemy prefab
    public Transform[] spawnPoints; // Array of spawn points
    public float spawnInterval = 5f; // Time between spawns
    public float triggerDistance = 30f; // Distance to trigger spawn
    private bool[] hasSpawned; // To track spawn status at each point

    void Start()
    {
        // Initialize spawn status
        hasSpawned = new bool[spawnPoints.Length];
        for (int i = 0; i < hasSpawned.Length; i++)
        {
            hasSpawned[i] = false;
        }

        // Check for sheep proximity at regular intervals
        InvokeRepeating(nameof(CheckSheepProximity), 0f, 0.5f); // Adjust interval as needed
    }

    void CheckSheepProximity()
    {
        // Find all sheep in the scene
        GameObject[] sheep = GameObject.FindGameObjectsWithTag("Sheep");

        foreach (GameObject singleSheep in sheep)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                // Check distance and if not yet spawned
                if (!hasSpawned[i] && Vector3.Distance(singleSheep.transform.position, spawnPoints[i].position) <= triggerDistance)
                {
                    SpawnEnemy(spawnPoints[i]);
                    hasSpawned[i] = true; // Mark spawn point as used
                }
            }
        }
    }

    void SpawnEnemy(Transform spawnPoint)
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"Enemy spawned at {spawnPoint.position}");
    }
}