using JetBrains.Annotations;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace EE.QC
{
    public class QuadrupedController : MonoBehaviour
    {
        private TcpClient m_client;
        private StreamWriter m_writer;
        private StreamReader m_reader;

        private const string SERVER_ADDRESS = "127.0.0.1";
        private const int PORT = 5000;

        #region Unity Callbacks

        [UsedImplicitly]
        private async void Start()
        {
            await __M_ConnectToServerAsync();
        }

        [UsedImplicitly]
        private async void Update()
        {
            if (m_client is not { Connected: true })
            {
                Debug.LogError("Not connected to server.");
                return;
            }

            var dataToSend = DateTime.Now.ToString("HH:mm:ss.fff");
            await __M_SendDataAsync(dataToSend);

            var response = await __M_ReceiveDataAsync();
            if (response != null) Debug.Log("Response from .exe: " + response);
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            m_writer?.Close();
            m_reader?.Close();
            m_client?.Close();
        }

        #endregion

        #region Internal

        private async Task __M_ConnectToServerAsync()
        {
            try
            {
                m_client = new TcpClient();
                await m_client.ConnectAsync(SERVER_ADDRESS, PORT);
                m_writer = new StreamWriter(m_client.GetStream()) { AutoFlush = true };
                m_reader = new StreamReader(m_client.GetStream());
                Debug.Log("Connected to .exe server.");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to connect to server: " + e.Message);
            }
        }

        private async Task __M_SendDataAsync(string message)
        {
            try
            {
                await m_writer.WriteLineAsync(message);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to send data: " + e.Message);
            }
        }

        private async Task<string> __M_ReceiveDataAsync()
        {
            try
            {
                if (m_client.Available > 0)
                {
                    return await m_reader.ReadLineAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to receive data: " + e.Message);
            }

            return null;
        }

        #endregion
    }
}
