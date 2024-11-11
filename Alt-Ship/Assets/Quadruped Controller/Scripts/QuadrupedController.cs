using JetBrains.Annotations;
using System;
using System.IO;
using System.Net.Sockets;
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

        [UsedImplicitly]
        private void Start()
        {
            ConnectToServer();
        }

        [UsedImplicitly]
        private void Update()
        {
            if (m_client is not { Connected: true })
            {
                Debug.LogError("Not connected to server.");
                return;
            }

            var dataToSend = DateTime.Now.ToString("HH:mm:ss.fff");
            SendData(dataToSend);

            var response = ReceiveData();
            if (response != null)
            {
                Debug.Log("Response from .exe: " + response);
            }
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            m_writer?.Close();
            m_reader?.Close();
            m_client?.Close();
        }

        private void ConnectToServer()
        {
            try
            {
                m_client = new TcpClient(SERVER_ADDRESS, PORT);
                m_writer = new StreamWriter(m_client.GetStream()) { AutoFlush = true };
                m_reader = new StreamReader(m_client.GetStream());
                Debug.Log("Connected to .exe server.");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to connect to server: " + e.Message);
            }
        }

        private void SendData(string message)
        {
            try
            {
                m_writer.WriteLine(message);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to send data: " + e.Message);
            }
        }

        private string ReceiveData()
        {
            try
            {
                if (m_client.Available > 0) return m_reader.ReadLine();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to receive data: " + e.Message);
            }

            return null;
        }
    }
}