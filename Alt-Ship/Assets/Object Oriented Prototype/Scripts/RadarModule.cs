using System;
using UnityEngine;

namespace EE.Prototype.OOP
{
    public class RadarModule : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask m_detetableLayer;
        [SerializeField] private float m_raidus;

        private Vector3[] m_positions;
        public Vector3[] Positions => m_positions;
        public int Capacity { get; private set; }

        private void FixedUpdate()
        {
            Collider[] results = null;
            Physics.OverlapSphereNonAlloc(transform.position, m_raidus, results, m_detetableLayer);

            Capacity = results.Length;

            if (Capacity <= 0) return;

            if (Capacity > m_positions.Length)
            {
                Array.Resize(ref m_positions, m_positions.Length * 2);
            }

            for (int i = 0; i < Capacity; ++i)
            {
                var collider = results[i];
                m_positions[i] = collider.transform.position;
            }
        }
    }
}
