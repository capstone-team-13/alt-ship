using UnityEngine;

public class SeagullFlyer : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 2f;

    private Vector3 flightCenter;
    private Vector3 flightRange;
    private Vector3 targetPos;

    public void SetFlightArea(Vector3 center, Vector3 range)
    {
        flightCenter = center;
        flightRange = range;
        RespawnAtEdge(); // 初始就从边缘生成
    }

    void Update()
    {
        // 检查是否飞出边界
        if (!IsWithinBounds(transform.position))
        {
            RespawnAtEdge(); // 重生在边缘
            return;
        }

        // 到达目标 → 选新目标
        if (Vector3.Distance(transform.position, targetPos) < 1f)
        {
            PickNewTarget();
        }

        // 朝目标方向飞
        Vector3 dir = targetPos - transform.position;
        dir.y = 0; // 保持平飞（如需上下飞可注释掉）
        dir.Normalize();

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void PickNewTarget()
    {
        // 生成靠边一点的随机点
        Vector3 offset = new Vector3(
            Random.Range(-flightRange.x, flightRange.x),
            Random.Range(0, flightRange.y),
            Random.Range(-flightRange.z, flightRange.z)
        );

        // 让它尽量接近边界（比如偏移X或Z方向放大）
        if (Random.value > 0.5f)
            offset.x = Mathf.Sign(offset.x) * flightRange.x;
        else
            offset.z = Mathf.Sign(offset.z) * flightRange.z;

        targetPos = flightCenter + offset;
    }

    void RespawnAtEdge()
    {
        Vector3 spawnOffset = new Vector3(
            Random.Range(-1f, 1f) * flightRange.x,
            Random.Range(0, flightRange.y),
            Random.Range(-1f, 1f) * flightRange.z
        );

        // 把其中一个轴拉到边缘
        if (Random.value > 0.5f)
            spawnOffset.x = Mathf.Sign(spawnOffset.x) * flightRange.x;
        else
            spawnOffset.z = Mathf.Sign(spawnOffset.z) * flightRange.z;

        transform.position = flightCenter + spawnOffset;
        PickNewTarget();
    }

    bool IsWithinBounds(Vector3 pos)
    {
        Vector3 localPos = pos - flightCenter;
        return Mathf.Abs(localPos.x) <= flightRange.x &&
               localPos.y >= 0 && localPos.y <= flightRange.y &&
               Mathf.Abs(localPos.z) <= flightRange.z;
    }
}
