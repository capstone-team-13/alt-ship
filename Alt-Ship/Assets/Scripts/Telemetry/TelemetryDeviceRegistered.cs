using Boopoo.Telemetry;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TelemetryDeviceRegistered : MonoBehaviour
{
    [System.Serializable]
    public struct TelemetryDeviceData
    {
        public string deviceList;

        public TelemetryDeviceData(string deviceList)
        {
            this.deviceList = deviceList;
        }
    }

    public void Log()
    {
        if (PlayerDeviceManager.Instance == null) return;

        var instance = PlayerDeviceManager.Instance;
        var playerCount = instance.MaxPlayerCount;
        if (playerCount <= 0) return;

        List<string> deviceTypes = new();
        for (var id = 0; id < playerCount; id++)
        {
            var device = instance.GetDeviceByPlayerId(id);
            var deviceType = device switch
            {
                Gamepad => "Controller",
                Keyboard => "Keyboard",
                _ => "Unknown"
            };
            deviceTypes.Add(deviceType);
        }

        TelemetryLogger.Log(this, "Player Input Devices", new TelemetryDeviceData(string.Join(",", deviceTypes)));
    }
}