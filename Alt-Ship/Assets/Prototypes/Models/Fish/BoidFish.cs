using UnityEngine;

public class BoidFish : MonoBehaviour
{
    [Header("游动参数")]
    public float speed = 1.5f;
    public float rotationSpeed = 2f;
    public float neighborDistance = 5f;
    public float separationDistance = 2f;

    private Vector3 moveDirection;
    private FishManager manager;

    void Start()
    {
        moveDirection = transform.forward;
        manager = GetComponentInParent<FishManager>();
    }

    void Update()
    {
        // ✅ 若速度为0，则不移动也不更新方向（可改为允许转头）
        if (speed <= 0.01f)
            return;

        GameObject[] allFish = GameObject.FindGameObjectsWithTag("Fish");

        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (GameObject other in allFish)
        {
            if (other == this.gameObject) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);

            if (distance < neighborDistance)
            {
                cohesion += other.transform.position;
                alignment += other.transform.forward;
                count++;

                if (distance < separationDistance)
                {
                    separation += transform.position - other.transform.position;
                }
            }
        }

        Vector3 direction = transform.forward;

        if (count > 0)
        {
            cohesion = ((cohesion / count) - transform.position).normalized;
            alignment = (alignment / count).normalized;
            separation = separation.normalized;

            direction += cohesion * 1.0f + alignment * 1.0f + separation * 1.5f;
        }

        // ✅ 加入中心控制，避免游出范围
        Vector3 centerOffset = manager.GetSwimCenter() - transform.position;
        if (centerOffset.magnitude > manager.GetSwimRange())
        {
            direction += centerOffset.normalized * 2f;
        }

        // ✅ 限制在XZ平面（清空Y方向）
        direction.y = 0f;
        direction.Normalize();

        // ✅ 平滑转向
        moveDirection = Vector3.Lerp(moveDirection, direction, rotationSpeed * Time.deltaTime);

        // ✅ 只在平面内旋转
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ✅ 移动
        transform.position += moveDirection * speed * Time.deltaTime;

        // ✅ 固定Y坐标（保持在平面）
        Vector3 pos = transform.position;
        pos.y = manager.GetSwimCenter().y; // 或者直接设为 y = 0
        transform.position = pos;
    }
}
