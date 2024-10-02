using UnityEngine;

namespace EE.Prototype.CC
{
    public abstract class Joint : MonoBehaviour
    {
        [SerializeField] private float m_mass;

        public abstract JointType Type { get; }

        public float Mass
        {
            get => m_mass;
            set => m_mass = value;
        }

        public abstract int DegreeOfFreedom { get; }
    }
}