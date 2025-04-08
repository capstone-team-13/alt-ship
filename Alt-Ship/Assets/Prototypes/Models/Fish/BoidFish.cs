using UnityEngine;

public class BoidFish : MonoBehaviour
{
    [Header("setting")]
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

     
        Vector3 centerOffset = manager.GetSwimCenter() - transform.position;
        if (centerOffset.magnitude > manager.GetSwimRange())
        {
            direction += centerOffset.normalized * 2f;
        }

     
        direction.y = 0f;
        direction.Normalize();

    
        moveDirection = Vector3.Lerp(moveDirection, direction, rotationSpeed * Time.deltaTime);

     
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    
        transform.position += moveDirection * speed * Time.deltaTime;

     
        Vector3 pos = transform.position;
        pos.y = manager.GetSwimCenter().y; 
        transform.position = pos;
    }
}
