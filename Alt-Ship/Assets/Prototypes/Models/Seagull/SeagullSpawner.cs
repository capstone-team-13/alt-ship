using UnityEngine;

public class SeagullSpawner : MonoBehaviour
{
    [Header("Seagull setting")]
    public GameObject seagullPrefab;
    public int seagullCount = 10;
    public Vector3 spawnRange = new Vector3(50, 10, 50); 

    void Start()
    {
        for (int i = 0; i < seagullCount; i++)
        {
            Vector3 pos = transform.position;
            pos += new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            Random.Range(0, spawnRange.y),
            Random.Range(-spawnRange.z, spawnRange.z)
            );

            GameObject g = Instantiate(seagullPrefab, pos, Quaternion.identity, transform);
            g.AddComponent<SeagullFlyer>().SetFlightArea(transform.position, spawnRange);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + Vector3.up * (spawnRange.y / 2f), new Vector3(spawnRange.x * 2, spawnRange.y, spawnRange.z * 2));
    }
}
