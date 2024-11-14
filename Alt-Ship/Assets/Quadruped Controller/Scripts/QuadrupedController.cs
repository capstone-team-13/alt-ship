using ENet;
using JetBrains.Annotations;
using System;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace EE.QC
{
    public class Logger
    {
        public static void LogError(object error)
        {
            Debug.LogError($"[{DateTime.Now:HH:mm:ss}] {error}");
        }

        public static void Log(object info)
        {
            Debug.Log($"[{DateTime.Now:HH:mm:ss}] {info}");
        }
    }

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
            m_server.DisconnectNow(0);
            m_client.Dispose();
            Library.Deinitialize();
        }

        #endregion

        #region Internal

        private Host m_client;
        private Peer m_server;

        private const string SERVER_ADDRESS = "127.0.0.1";
        private const int PORT = 5000;

        private const int BUFFER_SIZE = 1024;
        private byte[] m_buffer;
        private MemoryStream m_readStream;
        private BinaryReader m_reader;

        private void __M_InitializeENet()
        {
            try
            {
                Application.runInBackground = true;
                var initialized = Library.Initialize();

                if (!initialized)
                {
                    Logger.LogError("ENet Library initialization failed.");
                    throw new InvalidOperationException("ENet Library initialization failed.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception occurred during ENet Library initialization: {ex.Message}");
                Logger.LogError("Stack Trace: " + ex.StackTrace);
                throw;
            }

            try
            {
                m_client = new Host();
                m_client.Create();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to create ENet client host: " + ex.Message);
                return;
            }

            var address = new Address();
            address.SetHost(SERVER_ADDRESS);
            address.Port = PORT;

            m_server = m_client.Connect(address);
            m_server.Timeout(5, 500, 2000);
        }

        private void __M_CreateBufferReader()
        {
            m_buffer = new byte[BUFFER_SIZE];
            m_readStream = new MemoryStream(m_buffer);
            m_reader = new BinaryReader(m_readStream);
        }

        private void __M_HandleEvents()
        {
            try
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
                        Logger.Log($"Connected to server - PID: {m_server.ID}, IP: {m_server.IP}");
                        break;

                    case ENet.EventType.Disconnect:
                        Logger.Log($"Disconnected from server - PID: {m_server.ID}");
                        break;

                    case ENet.EventType.Timeout:
                        Logger.LogError($"Connection timeout - PID: {m_server.ID}, RTT: {m_server.RoundTripTime} ms");
                        Logger.Log(m_server.State);
                        break;

                    case ENet.EventType.Receive:
                        try
                        {
                            __M_ParsePacket(ref netEvent);
                        }
                        finally
                        {
                            netEvent.Packet.Dispose();
                        }

                        break;

                    default:
                        Logger.LogError($"Unhandled event type: {netEvent.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception occurred while handling events: {ex.Message}");
            }
        }

        private void __M_ParsePacket(ref ENet.Event netEvent)
        {
            m_readStream.Position = 0;
            netEvent.Packet.CopyTo(m_buffer);
            var packetType = (PacketType)m_reader.ReadByte();
            Logger.Log(
                $"Packet received - Type: {packetType}, Length: {netEvent.Packet.Length} bytes, Channel ID: {netEvent.ChannelID}");

            switch (packetType)
            {
                case PacketType.ConnectionSucceed:
                    var remainingBytes = m_reader.ReadBytes(netEvent.Packet.Length - 1);
                    var response = System.Text.Encoding.UTF8.GetString(remainingBytes);
                    Logger.Log($"{response}, len: {response.Length}");
                    break;

                case PacketType.PositionUpdate:
                    if (m_reader.BaseStream.Length - m_reader.BaseStream.Position >= 12)
                    {
                        var position = new Vector3(m_reader.ReadSingle(), m_reader.ReadSingle(), m_reader.ReadSingle());
                        __M_UpdatePosition(position);
                    }
                    else
                    {
                        Logger.LogError("Not enough bytes available to read a Vector3 position.");
                    }

                    break;

                default:
                    try
                    {
                        m_reader.BaseStream.Position = 0;
                        var unknownData = m_reader.ReadString();
                        Logger.LogError($"Unknown event type. Attempted to read data as string: {unknownData}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Unknown event type and failed to read data as string: {ex.Message}");
                    }

                    throw new ArgumentOutOfRangeException(nameof(packetType), $"Unknown packet type: {packetType}");
            }
        }

        private void __M_UpdatePosition(Vector3 position)
        {
            m_body.transform.position = position;
        }

        #endregion
    }
}