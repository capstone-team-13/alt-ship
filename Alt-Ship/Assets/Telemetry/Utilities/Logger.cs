using System;
using UnityEngine;
using static Boopoo.Telemetry.TelemetrySetting;

namespace Boopoo.Telemetry
{
    public static class Logger
    {
        #region API

        public static LoggingLevel LoggingLevel { get; set; } = LoggingLevel.Verbose;
        public static void LogVerbose(string message) => Log(message, LoggingLevel.Verbose, Debug.Log);
        public static void LogConnection(string message) => Log(message, LoggingLevel.Connections, Debug.Log);
        public static void LogWarning(string message) => Log(message, LoggingLevel.Warnings, Debug.LogWarning);
        public static void LogError(string message) => Log(message, LoggingLevel.Errors, Debug.LogError);

        private static void Log(string message, LoggingLevel level, Action<string> logAction)
        {
            if (LoggingLevel <= level) logAction($"{PREFIX} {message}");
        }

        public static void Disable() => LoggingLevel = LoggingLevel.Off;

        #endregion

        #region Internal

        private const string PREFIX = "[Telemetry] ";

        #endregion
    }
}