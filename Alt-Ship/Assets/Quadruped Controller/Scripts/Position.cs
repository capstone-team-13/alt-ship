using UnityEngine;

namespace EE.QC
{
    public class Position
    {
        public Vector3 Value { get; private set; }

        public Position(float x, float y, float z)
        {
            Value = new Vector3(x, z, -y);
        }

        public static implicit operator Vector3(Position position) => position.Value;

        public override string ToString() => Value.ToString();
    }
}