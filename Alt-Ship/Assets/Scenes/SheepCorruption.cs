using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepCorruption : MonoBehaviour
{
    public float corruptionTime = 5f; // Time to corrupt sheep
    private float currentCorruption = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(CorruptSheep(other.gameObject));
        }
    }

    IEnumerator CorruptSheep(GameObject enemy)
    {
        while (currentCorruption < corruptionTime)
        {
            currentCorruption += Time.deltaTime;
            // Add visual feedback here (e.g., change color gradually)
            yield return null;
        }

        // Handle sheep being taken
        Debug.Log("Sheep has been corrupted!");
        Destroy(enemy); // Remove enemy after corruption
    }
}
