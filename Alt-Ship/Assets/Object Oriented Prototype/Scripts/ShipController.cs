using UnityEngine;

namespace EE.Prototype.OOP
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Vector3 direction;

        private void Start()
        {
            direction.Normalize();
        }

        private void Update()
        {
            transform.position += speed * direction * Time.deltaTime;
        }
    }
}
