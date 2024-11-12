using ENet;
using JetBrains.Annotations;
using System;
using System.IO;
using UnityEngine;

namespace EE.QC
{
    public class QuadrupedController : MonoBehaviour
    {
        #region Editor API

        [SerializeField] private Transform m_body;

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void Start()
        {
            __M_InitializeENet();
            __M_CreateBufferReader();
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            __M_HandleEvents();
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            m_client.Dispose();
            Library.Deinitialize();
        }

        #endregion

        #region Internal

        private Host m_client;
        private Peer m_peer;

        private const string SERVER_ADDRESS = "127.0.0.1";
        private const int PORT = 5000;

        private const int BUFFER_SIZE = 1024;
        private byte[] m_buffer;
        private MemoryStream m_readStream;
        private BinaryReader m_reader;

        private void __M_InitializeENet()
        {
            Application.runInBackground = true;
            Library.Initialize();

            m_client = new Host();
            var address = new Address();
            address.SetHost(SERVER_ADDRESS);
            address.Port = PORT;
            m_client.Create();

            m_peer = m_client.Connect(address);
        }

        private void __M_CreateBufferReader()
        {
            m_buffer = new byte[BUFFER_SIZE];
            m_readStream = new MemoryStream(m_buffer);
            m_reader = new BinaryReader(m_readStream);
        }

        private void __M_HandleEvents()
        {
            var noImmediateEvent = m_client.CheckEvents(out var netEvent) <= 0;
            if (noImmediateEvent)
            {
                var noEventAfterWait = m_client.Service(15, out netEvent) <= 0;
                if (noEventAfterWait) return;
            }

            switch (netEvent.Type)
            {
                case ENet.EventType.None:
                    break;

                case ENet.EventType.Connect:
                    __M_Log($"Connected to server - PID: {m_peer.ID}, IP: {m_peer.IP}");
                    break;

                case ENet.EventType.Disconnect:
                    __M_Log($"Disconnected from server - PID: {m_peer.ID}");
                    break;

                case ENet.EventType.Timeout:
                    __M_LogError($"Connection timeout - PID: {m_peer.ID}, RTT: {m_peer.RoundTripTime} ms");
                    break;

                case ENet.EventType.Receive:
                    __M_ParsePacket(ref netEvent);
                    netEvent.Packet.Dispose();
                    break;

                default:
                    __M_LogError($"Unhandled event type: {netEvent.Type}");
                    break;
            }
        }

        private void __M_ParsePacket(ref ENet.Event netEvent)
        {
            m_readStream.Position = 0;
            netEvent.Packet.CopyTo(m_buffer);
            var packetType = (PacketType)m_reader.ReadByte();
            __M_Log(
                $"Packet received - Type: {packetType}, Length: {netEvent.Packet.Length} bytes, Channel ID: {netEvent.ChannelID}");

            switch (packetType)
            {
                case PacketType.PositionUpdate:
                    if (m_reader.BaseStream.Length - m_reader.BaseStream.Position >= 12)
                    {
                        var position = new Vector3(m_reader.ReadSingle(), m_reader.ReadSingle(), m_reader.ReadSingle());
                        __M_UpdatePosition(position);
                    }
                    else __M_LogError("Not enough bytes available to read a Vector3 position.");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void __M_UpdatePosition(Vector3 position)
        {
            m_body.transform.position = position;
        }

        private static void __M_Log(string info)
        {
            Debug.Log($"[{DateTime.Now:HH:mm:ss}] " + info);
        }

        private static void __M_LogError(string error)
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss}] " + error);
        }

        #endregion
    }
}