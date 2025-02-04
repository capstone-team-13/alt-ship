using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boopoo.Telemetry
{
    /// <summary>
    /// Manages telemetry setting and logging behavior for the application.
    /// Allows configuration of telemetry data transmission and console logging levels.
    /// </summary>
    [CreateAssetMenu(fileName = "Telemetry Setting.asset", menuName = "Telemetry/Telemetry Setting")]
    public class TelemetrySetting : ScriptableObject
    {
        /// <summary>
        /// Defines the modes for transmitting telemetry data.
        /// </summary>
        public enum TransmissionMode
        {
            // Telemetry data is only logged locally and not sent to the server.
            LocalOnly,

            // Telemetry data is sent to the server based on specific runtime conditions or triggers.
            OnDemand,

            // Telemetry data is always sent to the server, regardless of conditions.
            Always,
        }

        /// <summary>
        /// Defines the levels of detail for logging to the Unity console.
        /// </summary>
        public enum LoggingLevel
        {
            // All messages, including detailed info, connections, warnings, and errors, are logged.
            Verbose,

            // Only logs related to connections are logged.
            Connections,

            // Only warnings are logged.
            Warnings,

            // Only errors are logged.
            Errors,

            // Logging is turned off entirely.
            Off,
        }

        [Tooltip(
            "This is used as the placeholder for the user name; actual usage will be determined in future versions.")]
        public string UserName = "Will use later, not this test";

        [Tooltip(
            "This is used as the placeholder for the password; actual usage will be determined in future versions.")]
        public string Password = "Will use later, not this test";

        [SerializeField]
        [Tooltip(
            "Specifies the version of the telemetry setting. Used for tracking changes and compatibility.")]
        private string m_version = "1.0";

        /// <summary>
        /// Gets the version text, automatically appended with the platform it's running on.
        /// This property formats the version string to include the application's running platform for easier identification.
        /// </summary>
        public string Version => $"{m_version}-{Application.platform}";

        [SerializeField]
        [Tooltip(
            "The URL of the server where telemetry data will be sent. Must be configured for telemetry transmission.")]
        private string m_serverURL = string.Empty;

        public string ServerURL => m_serverURL;

        [Tooltip("Allows you to configure the telemetry data transmission mode: LocalOnly, OnDemand, or Always.")]
        public TransmissionMode transmissionMode;

        [Tooltip("Controls the verbosity of messages that the telemetry scripts will print to the Unity console.")]
        public LoggingLevel loggingLevel;

#if UNITY_EDITOR
        /// <summary>
        /// Convenience method to quickly find the TelemetrySettings asset in the project. 
        /// Intended for use in the Unity Editor to simplify access to the setting.
        /// Note: If multiple setting assets exist, this method does not guarantee which one will be returned.
        /// </summary>
        /// <returns>The first settings asset found in the project.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when no Telemetry setting asset is found.</exception>
        public static TelemetrySetting GetDefault()
        {
            const string typeName = nameof(TelemetrySetting);
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");

            switch (guids.Length)
            {
                case > 1:
                    Debug.LogWarning(
                        $"{guids.Length} Telemetry setting found - double-check that this logger is using the one you intend.");
                    break;
                case 0:
                    throw new System.IO.FileNotFoundException(
                        "Could not find Telemetry setting asset. Be sure to create one.");
            }

            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<TelemetrySetting>(path);
        }

        private void OnValidate()
        {
            m_serverURL = m_serverURL.TrimEnd('/');
        }
#endif
    }
}