using ENet;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Profiling;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace EE.QC
{
    public class Logger
    {
        public static void LogError(params object[] errors)
        {
            string message = string.Join(" ", errors);
            Debug.LogError($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public static void Log(params object[] infos)
        {
            string message = string.Join(" ", infos);
            Debug.Log($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }

    public class QuadrupedController : MonoBehaviour
    {
        #region Editor API

        [SerializeField] private Transform m_body;
        [SerializeField] private LegReference[] m_legs;
        [SerializeField] private Transform[] m_endEffectors;

        #endregion

        #region Unity Callbacks

        [UsedImplicitly]
        private void Start()
        {
            __M_InitializeENet();
            __M_CreateBufferReader();

            for (var i = 0; i < 4; i++)
            {
                m_frames.Add(new LegFrame());
            }
        }

        [UsedImplicitly]
        private void Update()
        {
            const KeyCode keyCode = KeyCode.Space;
            if (Input.GetKeyDown(keyCode))
            {
                Logger.Log($"{keyCode} pressed.");
                // var packet = new Packet();
                // var data = new[] { (byte)EventType.ADD_FORCE };
                // packet.Create(data, data.Length, PacketFlags.Unsequenced);
                // m_server.Send(0, ref packet);
            }

            for (var i = 0; i < m_legs.Length; i++)
            {
                m_legs[i].Tick(m_frames[i]);
            }
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

        private List<LegFrame> m_frames = new(4);

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
            m_server.Timeout(5000, 10000, 30000);
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
                int eventSize = m_client.CheckEvents(out var netEvent);
                var noImmediateEvent = eventSize <= 0;
                if (noImmediateEvent)
                {
                    var noEventAfterWait = m_client.Service(1, out netEvent) <= 0;
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
            var packetType = (EventType)m_reader.ReadByte();

            switch (packetType)
            {
                case EventType.CONNECTION_SUCCEED:
                    var remainingBytes = m_reader.ReadBytes(netEvent.Packet.Length - 1);
                    var response = System.Text.Encoding.UTF8.GetString(remainingBytes);
                    Logger.Log($"Connection Succeed: {response}");
                    break;

                // if (m_reader.BaseStream.Length - m_reader.BaseStream.Position >= 14)
                case EventType.FL_UPDATE:
                    m_frames[0].Update(m_reader);
                    break;

                case EventType.BL_UPDATE:
                    m_frames[1].Update(m_reader);
                    break;

                case EventType.BR_UPDATE:
                    m_frames[2].Update(m_reader);
                    break;

                case EventType.FR_UPDATE:
                    m_frames[3].Update(m_reader);
                    break;

                case EventType.END_EFFECTOR_UPDATE:
                    foreach (var endEffector in m_endEffectors)
                    {
                        endEffector.position = new Vector3(m_reader.ReadSingle(),
                            m_reader.ReadSingle(), m_reader.ReadSingle());
                    }

                    break;

                case EventType.BODY_UPDATE:
                    m_body.position = new Vector3(m_reader.ReadSingle(), m_reader.ReadSingle(),
                        m_reader.ReadSingle());
                    m_body.rotation = new Rotation(m_reader.ReadSingle(), m_reader.ReadSingle(),
                        m_reader.ReadSingle(),
                        m_reader.ReadSingle());
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

        #endregion
    }
}