using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Boopoo.Telemetry.TelemetrySetting;

namespace Boopoo.Telemetry
{
    public class Service : MonoBehaviour
    {
        #region Editor API

        [SerializeField] public event Action<int> OnConnectionEstablished;
        [SerializeField] public event Action<string> OnDisconnected;

        #endregion

        #region API

        public enum ServiceStatus
        {
            NotInitialized, // Slightly more explicit than Uninitialized, indicating the service hasn't been initialized yet.
            Initializing, // This could be a clearer term than Waking, indicating the service is in the process of starting up.
            Authenticating, // Unchanged, clearly indicates the service is in the process of authenticating.
            Operational, // A bit broader than Ready, indicating the service is fully operational and ready to use.
            Disconnected // Unchanged, clearly indicates the service is not currently connected.
        }

        public bool AllowNetworkTransmission { get; private set; }

        public SessionInfo SessionInfo => m_sessionInfo;

        public static Service Connect(TelemetryLogger logger, Action<int> onConnectionSuccess,
            Action<string> onConnectionFailure)
        {
            if (logger.telemetrySetting == null)
            {
                Logger.LogError($"Telemetry logger {logger.name} has no Telemetry TelemetrySetting assigned.");
                return null;
            }

            if (m_currentService != null) m_currentService.Disconnect();

            if (!m_settingsToServiceMap.TryGetValue(logger.telemetrySetting, out var service))
            {
                service = new GameObject("Telemetry Service").AddComponent<Service>();
                DontDestroyOnLoad(service.gameObject);
                service.Initialize(logger.telemetrySetting);
                m_settingsToServiceMap.Add(logger.telemetrySetting, service);
                m_currentService = service;

                service.m_sceneName = logger.gameObject.scene.name;
                // Add listeners before connecting to avoid missing events triggered on connection.
                service.InitializeListeners(onConnectionSuccess, onConnectionFailure);
                service.ConnectTo(logger);
            }
            else
            {
                m_currentService = service;
                // Add listeners before connecting to avoid missing events triggered on connection.
                service.InitializeListeners(onConnectionSuccess, onConnectionFailure);
                service.RequestServiceFor(logger);
            }

            return service;
        }

        public void RequestServiceFor(TelemetryLogger logger)
        {
            if (logger.RegionName == m_lastRegionName) return;

            var telemetry = new Event<string>("Region Switched", logger.RegionName)
            {
                location = m_lastRegionName,
                sessionId = m_sessionInfo.id ,
                sessionKey = m_sessionInfo.key
            };
            TryLog(logger, telemetry);
            m_lastRegionName = logger.RegionName;
        }

        public void TryLog<T>(TelemetryLogger logger, Event<T> telemetry)
        {
            var data = JsonUtility.ToJson(telemetry);

            if (telemetry.data == null) data = JSONHelper.PurgeData(data);

#if UNITY_EDITOR
            else if (JSONHelper.IsInvalidData(telemetry, data))
            {
                Logger.LogError(
                    $"Trying to log event '{telemetry.name}' with non-serializable data payload type {typeof(T)}. Be sure to use the [System.Serializable] attribute on any custom class/struct you want to use as telemetry data.");
            }
#endif
            m_requestQueue.Enqueue(data);

            // TODO: Implement a rate limit to prevent excessive server requests.  
            // Only flush logs when the buffer is full or at controlled intervals.
            FlushLogsToServer();
        }

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void OnDestroy()
        {
            Disconnect();

            OnConnectionEstablished = null;
            OnDisconnected = null;
        }

        #endregion

        #region Internal

        private static Service m_currentService;
        private static readonly Dictionary<TelemetrySetting, Service> m_settingsToServiceMap = new();

        private TelemetrySetting m_telemetrySetting;

        private ServiceStatus m_serviceStatus = ServiceStatus.NotInitialized;
        private SessionInfo m_sessionInfo;

        private string m_sceneName;
        private float m_lastMessageTime;
        private string m_lastRegionName;

        private readonly Queue<string> m_requestQueue = new();
        private int m_requestBuffered = 0;

        private const int MAX_LOGS_PER_SECOND = 60;
        private const float MIN_SECONDS_BETWEEN_LOGS = 1f / MAX_LOGS_PER_SECOND;
        private const int RATE_WARNING_THRESHOLD = 2 * MAX_LOGS_PER_SECOND;

        private void InitializeListeners(Action<int> onConnectionSuccess, Action<string> onConnectionFailure)
        {
            OnConnectionEstablished += Service_OnConnectionEstablished;
            OnConnectionEstablished += onConnectionSuccess;
            OnDisconnected += onConnectionFailure;
        }

        private void Initialize(TelemetrySetting telemetrySetting)
        {
            m_telemetrySetting = telemetrySetting;
            Logger.LoggingLevel = m_telemetrySetting.loggingLevel;

            AllowNetworkTransmission = m_telemetrySetting.transmissionMode switch
            {
                TransmissionMode.Always => true,
                TransmissionMode.OnDemand => Application.isEditor == false,
                TransmissionMode.LocalOnly => false,
                _ => false
            };

            var message = $"Loaded telemetrySetting from '{telemetrySetting.name}': " +
                          $"{(AllowNetworkTransmission ? "<color=green>Logging to server enabled</color>" : "<color=yellow>Logging to local debug console only</color>")}.";
            Logger.LogConnection(message);
        }

        private void ConnectTo(TelemetryLogger logger)
        {
            m_lastRegionName = logger.RegionName;
            InitializeSession();
        }

        private void InitializeSession()
        {
            if (!AllowNetworkTransmission) InitializeOfflineSession();
            else StartCoroutine(StartInitializeOnlineSession());
        }

        private void InitializeOfflineSession()
        {
            var id = -1;
            var key = string.Empty;
            m_sessionInfo = new SessionInfo(id, key);

            AllowNetworkTransmission = false;
            m_serviceStatus = ServiceStatus.Operational;

            m_currentService.OnConnectionEstablished?.Invoke(m_sessionInfo.id);
        }

        private void Disconnect()
        {
            var message = $"Service in \"{m_sceneName}\" safely disconnected.";
            Logger.LogConnection(message);
            OnDisconnected?.Invoke(message);
        }

        private IEnumerator LogToServer(string url)
        {
            var data = m_requestQueue.Dequeue();

            m_lastMessageTime = Time.realtimeSinceStartup;

            var request = NetworkUtility.Post(url, data);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Logger.LogError($"Failed to log event: {NetworkUtility.ExtractErrorMessage(request)}\n{data}");
            else Logger.LogVerbose($"Successfully logged event: {data}");

            float secondsSinceLastMessage = Time.realtimeSinceStartup - m_lastMessageTime;
            yield return new WaitForSecondsRealtime(MIN_SECONDS_BETWEEN_LOGS - secondsSinceLastMessage);
        }

        private void FlushLogsToServer()
        {
            if (AllowNetworkTransmission && m_serviceStatus != ServiceStatus.Operational)
            {
                Logger.LogError("Have not connect to server yet, nothing will be logged");
                return;
            }

            var telemetrySetting = m_currentService.m_telemetrySetting;
            var url = $"{telemetrySetting.ServerURL}/api/events/create-event";

            while (m_requestQueue.Count > 0) StartCoroutine(LogToServer(url));
        }

        private IEnumerator StartInitializeOnlineSession()
        {
            m_serviceStatus = ServiceStatus.Initializing;
            // TODO: implement initializing logic

            m_serviceStatus = ServiceStatus.Authenticating;
            // TODO: implement authenticating logic

            StartCoroutine(StartRequestSessionInfo());
            yield return null;
        }

        private IEnumerator StartRequestSessionInfo()
        {
            var telemetrySetting = m_currentService.m_telemetrySetting;
            var url = $"{telemetrySetting.ServerURL}/api/sessions/create-session";
            Logger.LogConnection($"Version: {telemetrySetting.Version}");

            var request = NetworkUtility.Post(url, $"{{\"build_version\":\"{telemetrySetting.Version}\"}}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = request.downloadHandler.text;
                var validResponse = JSONHelper.TryParseJson(json, out SessionInfo sessionInfo);
                if (validResponse)
                {
                    m_sessionInfo = sessionInfo;
                    m_serviceStatus = ServiceStatus.Operational;
                    m_currentService.OnConnectionEstablished?.Invoke(m_sessionInfo.id);
                }
                else
                {
                    m_serviceStatus = ServiceStatus.Disconnected;
                    m_currentService.OnDisconnected?.Invoke(
                        $"Invalid JSON response received: {request.downloadHandler.text}");
                    InitializeOfflineSession();
                }
            }
            else
            {
                var error = NetworkUtility.ExtractErrorMessage(request);
                Logger.LogError($"Failed request session info: {error}");
                m_serviceStatus = ServiceStatus.Disconnected;
                m_currentService.OnDisconnected?.Invoke(error);
                InitializeOfflineSession();
            }
        }

        #endregion

        #region Event Handlers

        private static void Service_OnConnectionEstablished(int sessionId)
        {
            Logger.LogConnection($"Connect to session {sessionId}.");
        }

        #endregion
    }
}