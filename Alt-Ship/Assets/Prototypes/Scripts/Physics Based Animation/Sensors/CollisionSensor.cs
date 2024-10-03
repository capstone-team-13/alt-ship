using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace EE.Prototype.PBA
{
    public class CollisionSensor : MonoBehaviour
    {
        public UnityEvent<Collision> OnCollided;

        [UsedImplicitly]
        private void OnCollisionEnter(Collision collision)
        {
            OnCollided?.Invoke(collision);
        }
    }
}