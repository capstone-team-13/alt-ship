using JetBrains.Annotations;
using UnityEngine;

namespace Boopoo.Telemetry
{
    public class TelemetryRegion : MonoBehaviour
    {
        #region Editor API

        [SerializeField] private string m_name = string.Empty;

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void OnTriggerEnter(Collider other)
        {
            TelemetryLogger.SwitchRegion(this, m_name);
        }

#if UNITY_EDITOR
        [UsedImplicitly]
        private void OnValidate()
        {
            gameObject.name = $"Telemetry Region - {m_name}";
        }
#endif

        #endregion
    }
}