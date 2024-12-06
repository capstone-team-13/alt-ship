using UnityEngine;

namespace EE.Prototype.PC
{
    public class LegManager : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_body;
        [SerializeField] private Transform[] m_legsStartingPoints = new Transform[2];
        [SerializeField] private Transform[] m_legsEndingPoints = new Transform[2];
        private int m_currentLegIndex = 0;

        private void FixedUpdate()
        {
            _M_ConstructTriangle();
        }

        private void _M_ConstructTriangle()
        {
        }
    }
}