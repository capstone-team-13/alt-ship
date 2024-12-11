using JetBrains.Annotations;
using UnityEngine;

namespace EE.Prototype.PC
{
    public class AlignWith : MonoBehaviour
    {
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_dampingFactor = 0.1f;

        [UsedImplicitly]
        private void FixedUpdate()
        {
            _M_AlignWithBodyRotation();
        }


        private void _M_AlignWithBodyRotation()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, m_target.rotation, m_dampingFactor);
        }
    }
}