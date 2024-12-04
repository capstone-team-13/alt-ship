using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace EE.Prototype.CM
{
    public class Scaler : MonoBehaviour
    {
        [SerializeField] private Transform m_start;
        [SerializeField] private Transform m_end;

        private Vector3 m_locale;

        [UsedImplicitly]
        private void Start()
        {
            m_locale = transform.localScale;
        }

        [UsedImplicitly]
        private void Update()
        {
            var yScale = Mathf.Abs(m_start.position.y - m_end.position.y);

            var newScale = m_locale;
            newScale.y = m_locale.y + yScale;
            transform.localScale = newScale;

            var direction = m_start.position - m_end.position;
            transform.up = direction.normalized;
        }
    }
}