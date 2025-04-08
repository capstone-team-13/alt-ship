using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [Header("Fish setting")]
    public GameObject[] fishPrefabs;     
    public int fishCount = 30;           
    public float spawnRadius = 10f;      
    public float swimAreaSize = 20f;     

    private List<GameObject> allFish = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < fishCount; i++)
        {
          
            GameObject randomFish = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

          
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = transform.position.y; 

         
            GameObject fish = Instantiate(randomFish, spawnPos, Quaternion.identity, transform);
            fish.tag = "Fish";

            allFish.Add(fish);
        }
    }

    public Vector3 GetSwimCenter()
    {
        return transform.position;
    }

    public float GetSwimRange()
    {
        return swimAreaSize;
    }
}
