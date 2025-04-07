using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [Header("鱼群设置")]
    public GameObject[] fishPrefabs;     // 🐟 不同种类的鱼 prefab 列表
    public int fishCount = 30;           // 总共生成几条鱼
    public float spawnRadius = 10f;      // 初始生成范围
    public float swimAreaSize = 20f;     // 鱼群活动区域范围

    private List<GameObject> allFish = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < fishCount; i++)
        {
            // ✅ 从数组中随机挑选一种鱼
            GameObject randomFish = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

            // ✅ 在 spawnRadius 范围内随机生成位置
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = transform.position.y; // 保证Y轴高度一致

            // ✅ 实例化鱼
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
