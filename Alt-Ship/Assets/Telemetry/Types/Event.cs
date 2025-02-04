using System;

namespace Boopoo.Telemetry
{
    [Serializable]
    public class Event<T>
    {
        public T data;
        public int sessionId = -1;
        public string sessionKey = null;
        public string location = null;
        public string name;

        public TimeLogger TimeLogger = new();

        public Event(string name, T data = default)
        {
            this.name = name;
            this.data = data;
            TimeLogger.Time = DateTimeOffset.UtcNow;
        }
    }

    public class TimeLogger
    {
        private long m_timeCode;

        public DateTimeOffset Time
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(m_timeCode);
            set => m_timeCode = value.ToUnixTimeMilliseconds();
        }
    }
}