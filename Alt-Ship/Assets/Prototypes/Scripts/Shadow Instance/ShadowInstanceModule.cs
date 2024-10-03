using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace EE.Prototype.SI
{
    public class ShadowInstanceModule : MonoBehaviour
    {
        [SerializeField] private Light m_light;
        [SerializeField] private Camera m_camera;
        [SerializeField] private RawImage m_image;

        private RenderTexture m_shadowMap;

        [UsedImplicitly]
        private void Awake()
        {
        }

        void OnDisable()
        {
            if (m_shadowMap != null)
            {
                m_shadowMap.Release();
                m_shadowMap = null;
            }
        }
    }
}