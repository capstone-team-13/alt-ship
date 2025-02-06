using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPref;

    [Header("Cloud Variables")]
    public int cloudCount = 20;
    public float spawnTime = 5f;
    public Vector3 spawnArea = new Vector3(1000, 500, 1000);
    public float minScale = 0f, maxScale = 2f;
    public float spawnAreaYMin = 250f;

    private int instanceCount = 0;

    void Start()
    {
        StartCoroutine(cloudSpawner());
    }

    IEnumerator cloudSpawner()
    {
        while (true)
        {
            if (instanceCount < cloudCount)
            {
                Vector3 randomPosition = new Vector3(
                    Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                    Random.Range(spawnAreaYMin, spawnArea.y),
                    Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
                    );

                float randomScale = Random.Range(minScale, maxScale);
                Vector3 scale = new Vector3(randomScale, randomScale, randomScale);

                GameObject cloud = Instantiate(cloudPref, randomPosition, Quaternion.identity);
                cloud.transform.localScale = cloud.transform.localScale + scale;

                instanceCount++;
                cloud.GetComponent<CloudEffects>().OnCloudDespawn += () => instanceCount--;
            }
            yield return new WaitForSeconds(spawnTime);
        }

    }
}
