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
        PickNewTarget();
    }

    void Update()
    {
        
        if (Vector3.Distance(transform.position, targetPos) < 1f)
        {
            PickNewTarget();
        }

      
        Vector3 dir = targetPos - transform.position;
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
        float x = Random.Range(-flightRange.x, flightRange.x);
        float y = Random.Range(0, flightRange.y);
        float z = Random.Range(-flightRange.z, flightRange.z);
        targetPos = flightCenter + new Vector3(x, y, z);
    }
}