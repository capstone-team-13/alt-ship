using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Boopoo.Telemetry
{
    public static class JSONHelper
    {
        #region API

        public static bool TryParseJson<T>(string json, out T result)
        {
            try
            {
                result = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error parsing JSON: {exception.Message}\n{json}");
                result = default;
                return false;
            }
        }

        public static string PurgeData(string json)
        {
            return m_dataPurge.Replace(json, "\"data\": null}");
        }

        public static bool IsInvalidData<T>(Event<T> source, string json)
        {
            return json.IndexOf("\"data\":", StringComparison.Ordinal) < 0;
        }

        #endregion

        #region Internal

        private static readonly Regex m_dataPurge = new("\"data\":.*}\\Z", RegexOptions.Compiled);

        #endregion
    }
}