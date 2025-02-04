using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Boopoo.Telemetry.TelemetrySetting;

namespace Boopoo.Telemetry
{
    public class TelemetryLogger : MonoBehaviour
    {
        #region Editor API

        [Header("Configs")] [Tooltip("Telemetry sent from this scene will use this label.")] [SerializeField]
        private string m_regionName;

        [SerializeField] private TelemetrySetting m_telemetrySetting;

        [Header("Callbacks")]
        [Space(8)]
        [Tooltip(
            "When successfully connected to the telemetry server, this will be called with the session ID number. Use this if you want to display the session number on-screen.")]
        public UnityEvent<int> OnConnectionEstablished;

        [Space(8)]
        [Tooltip(
            "When connection to the telemetry service fails, this will be called with the failure message. Use this if you want to print the error on-screen.")]
        public UnityEvent<string> OnDisconnected;

        #endregion

        #region API

        public TelemetrySetting telemetrySetting
        {
            get => m_telemetrySetting;
            private set => m_telemetrySetting = value;
        }

        public string RegionName => m_regionName;

        public Service Service { get; private set; }

        public void Log<T>(string eventType, T data)
        {
            var telemetry = new Event<T>(eventType, data)
            {
                sessionId = Service.SessionInfo.id,
                sessionKey = Service.SessionInfo.key,
                location = RegionName
            };
            Service.TryLog(this, telemetry);
        }

        public static void Log<T>(Scene source, string eventType, T data)
        {
            // Ask the selected logger to log this event.
            if (TryGetInstance(source, out var logger))
                logger.Log(eventType, data);
        }

        public static void Log<T>(Component sender, string eventType, T data)
        {
            var source = sender != null ? sender.gameObject.scene : default;
            Log(source, eventType, data);
        }

        public static void Log(Component sender, string eventType)
        {
            Log(sender, eventType, m_defaultData);
        }

        public void Log(string eventType)
        {
            Log(eventType, m_defaultData);
        }

        public static bool TryGetInstance(Scene source, out TelemetryLogger logger)
        {
            // If no scene was provided, use the current active scene.
            if (source == default) source = SceneManager.GetActiveScene();
            // Try to find the logger for that scene.
            if (m_instances.TryGetValue(source, out logger)) return true;

            // If there's none in our cache, check the target scene.
            Scene active = SceneManager.GetActiveScene();
            if (source != active) SceneManager.SetActiveScene(source);
            logger = FindObjectOfType<TelemetryLogger>();
            if (source != active) SceneManager.SetActiveScene(active);

            // If that fails, systematically search for *any* active logger we can use.
            if (logger == null)
            {
                foreach (TelemetryLogger value in m_instances.Values)
                {
                    if (value == null) continue;
                    logger = value;
                    break;
                }
            }

            // If all our attempts have failed, report an error and abort.
            if (logger == null)
            {
                Debug.LogError(
                    $"Trying to log telemetry in a scene '{source.name}' when no TelemetryLogger is available. Be sure to place a TelemetryLogger in your scene.");
                return false;
            }

            if (logger.gameObject.scene != source)
            {
                // If we found a logger, but it's mismatched, warn about it.
                Logger.LogWarning(
                    $"Trying to log telemetry in a scene '{source.name}' with no loggers. Redirecting to active logger from '{logger.gameObject.scene.name}' instead. This might lead to misleading 'section' values reported in your telemetry.");
            }

            return true;
        }

        public void SwitchRegion(string location)
        {
            if (!Service.AllowNetworkTransmission) return;

            m_regionName = location;
            Service.RequestServiceFor(this);
        }

        public static void SwitchRegion(Component sender, string location)
        {
            if (!TryGetInstance(sender.gameObject.scene, out var logger)) return;
            logger.SwitchRegion(location);
        }

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void Awake()
        {
            if (Service == null) Initialize();
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            if (m_instances.TryGetValue(gameObject.scene, out TelemetryLogger value) && value == this)
                m_instances.Remove(gameObject.scene);

            if (Service == null) return;
            Service.OnConnectionEstablished -= Service_OnConnectionEstablished;
            Service.OnDisconnected -= Service_OnDisconnected;
        }

#if UNITY_EDITOR
        [UsedImplicitly]
        private void OnValidate()
        {
            if (telemetrySetting == null) telemetrySetting = GetDefault();
        }
#endif

        #endregion

        #region Internal

        private static readonly DefaultData m_defaultData = default;
        private static readonly Dictionary<Scene, TelemetryLogger> m_instances = new();

        private void Initialize()
        {
            // Ensure there's at most one cached logger in the scene.
            if (m_instances.ContainsKey(gameObject.scene))
            {
                Logger.LogWarning(
                    $"Multiple {nameof(TelemetryLogger)} instances detected in scene '{gameObject.scene.name}'. " +
                    "Ensure each scene has a unique, correctly assigned instance.");
            }
            else m_instances.Add(gameObject.scene, this);

            // Fetch a telemetry service that matches our TelemetrySettings, spinning up a new one if needed.
            Service = Service.Connect(this, Service_OnConnectionEstablished, Service_OnDisconnected);
        }

        #endregion

        #region Event Handlers

        private void Service_OnConnectionEstablished(int sessionId)
        {
            OnConnectionEstablished?.Invoke(sessionId);
        }

        private void Service_OnDisconnected(string message)
        {
            OnDisconnected?.Invoke(message);
        }

        #endregion
    }
}