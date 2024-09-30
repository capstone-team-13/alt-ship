using System;
using UnityEngine;

namespace EE.Prototype.OOP
{
    public class RadarModule : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask m_detetableLayer;
        [SerializeField] private float m_raidus;

        private Collider[] m_results = new Collider[8];

        public float Radius => m_raidus;
        public Collider[] Results => m_results;
        public int HitCount { get; private set; }

        private void FixedUpdate()
        {
            HitCount = Physics.OverlapSphereNonAlloc(transform.position, m_raidus, m_results, m_detetableLayer);

            if (HitCount <= 0) return;

            if (HitCount > m_results.Length)
            {
                Array.Resize(ref m_results, HitCount); 
                Physics.OverlapSphereNonAlloc(transform.position, m_raidus, m_results, m_detetableLayer);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_raidus);
        }
    }
}
