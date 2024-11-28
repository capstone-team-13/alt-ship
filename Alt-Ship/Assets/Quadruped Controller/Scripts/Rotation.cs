using Quaternion = UnityEngine.Quaternion;

namespace EE.QC
{
    public struct Rotation 
    {
        private Quaternion quaternion;

        public Rotation(float w, float x, float y, float z)
        {
            quaternion = new Quaternion(x, y, z, w); 
        }

        public Quaternion Quaternion
        {
            get { return quaternion; }
            set { quaternion = value; }
        }

        public static implicit operator Quaternion(Rotation r)
        {
            return r.quaternion;
        }
    }
}
