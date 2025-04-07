using UnityEngine;

public class SharkController : MonoBehaviour
{
    [Header("鲨鱼设置")]
    public GameObject sharkPrefab;
    public float speed = 2f;
    public float rotationSpeed = 2f;

    [Header("活动范围")]
    public Vector3 range = new Vector3(20, 0, 20); // 只在XZ方向移动

    [Header("目标切换停顿")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 2f;

    private GameObject sharkInstance;
    private Vector3 targetPosition;

    private bool waiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 0f;

    void Start()
    {
        sharkInstance = Instantiate(sharkPrefab, transform.position, Quaternion.identity, transform);
        PickNewTarget();
    }

    void Update()
    {
        if (!sharkInstance) return;

        // 如果正在等待，就不动
        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                waiting = false;
                PickNewTarget();
            }
            return;
        }

        // 到达目标 → 进入等待状态
        if (Vector3.Distance(sharkInstance.transform.position, targetPosition) < 1f)
        {
            waiting = true;
            waitTimer = 0f;
            waitDuration = Random.Range(minWaitTime, maxWaitTime);
            return;
        }

        MoveShark();
    }

    void MoveShark()
    {
        Vector3 dir = targetPosition - sharkInstance.transform.position;
        dir.y = 0f; // 保持在平面内
        dir.Normalize();

        if (dir != Vector3.zero)
        {
            Vector3 smoothedDir = Vector3.Lerp(sharkInstance.transform.forward, dir, rotationSpeed * Time.deltaTime);
            Quaternion rot = Quaternion.LookRotation(smoothedDir);
            sharkInstance.transform.rotation = rot;
        }

        sharkInstance.transform.position += sharkInstance.transform.forward * speed * Time.deltaTime;

        // 固定在指定平面高度
        Vector3 pos = sharkInstance.transform.position;
        pos.y = transform.position.y;
        sharkInstance.transform.position = pos;
    }

    void PickNewTarget()
    {
        Vector3 center = transform.position;
        Vector3 newTarget;

        // 防止新目标太近（避免转圈）
        do
        {
            float x = Random.Range(-range.x, range.x);
            float z = Random.Range(-range.z, range.z);
            newTarget = new Vector3(center.x + x, center.y, center.z + z);
        }
        while (Vector3.Distance(newTarget, sharkInstance.transform.position) < 3f);

        targetPosition = newTarget;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, new Vector3(range.x * 2, 0.1f, range.z * 2));
    }
}
