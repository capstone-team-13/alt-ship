using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Boopoo.Telemetry
{
    public static class NetworkUtility
    {
        public static UnityWebRequest Get(string url)
        {
            var request = UnityWebRequest.Get(url);
            return request;
        }

        public static UnityWebRequest Post(string url, string json)
        {
            var request = UnityWebRequest.Put(url, json);

            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.timeout = 3;

            return request;
        }

        public static UnityWebRequest Post<T>(string url, T data)
        {
            var jsonString = JsonUtility.ToJson(data);
            return Post(url, jsonString);
        }

        /// <summary>
        /// Extracts and returns the error message from a UnityWebRequest instance. 
        /// If the downloadHandler contains text, it returns this text as the error message. 
        /// Otherwise, it returns the default error message from the request.
        /// </summary>
        /// <param name="request">The UnityWebRequest instance to extract the error message from.</param>
        /// <returns>The error message, or null if no error is found.</returns>
        public static string ExtractErrorMessage(UnityWebRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request), "The request cannot be null.");

            // Attempt to use downloadHandler text as the error message if available.
            string errorMessage = request.downloadHandler?.text;

            // If the downloadHandler text is empty or whitespace, use the default error message.
            if (string.IsNullOrWhiteSpace(errorMessage)) errorMessage = request.error;

            return errorMessage;
        }
    }
}