using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DeformableController : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Vector3 m_force;
    [SerializeField] private Material m_material;

    [UsedImplicitly]
    private void Update()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            transform.position = new Vector3(0, 5, 0);
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;

            Vector3 forceDirection = Camera.main.transform.forward;
            m_rigidbody.AddForce(forceDirection * 10.0f, ForceMode.Impulse);
        }
    }

    [UsedImplicitly]
    private void LateUpdate()
    {
        // TODO: Invalid in XY Plane
        transform.rotation = m_rigidbody.velocity.sqrMagnitude > Mathf.Epsilon
            ? Quaternion.LookRotation(m_rigidbody.velocity.normalized)
            : Quaternion.identity;

        float num = 0.8f;
        var x = math.remap(-10.0f, 10.0f, -num, num, m_rigidbody.velocity.x);
        var y = math.remap(-10.0f, 10.0f, -num, num, m_rigidbody.velocity.y);
        var z = math.remap(-10.0f, 10.0f, -num, num, m_rigidbody.velocity.z);
        var newDirection = new Vector3(x, y, z);

        m_material.SetVector("_MoveDirection", newDirection);
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
        m_material.SetVector("_MoveDirection", Vector4.zero);
    }

    // [UsedImplicitly]
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(transform.position, transform.forward * 2.0f);
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawRay(transform.position, m_rigidbody.velocity.normalized);
    // }
}