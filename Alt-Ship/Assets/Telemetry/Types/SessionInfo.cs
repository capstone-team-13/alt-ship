namespace Boopoo.Telemetry
{
    public struct SessionInfo
    {
        public int id;
        public string key;

        public SessionInfo(int id, string key)
        {
            this.id = id;
            this.key = key;
        }
    }
}