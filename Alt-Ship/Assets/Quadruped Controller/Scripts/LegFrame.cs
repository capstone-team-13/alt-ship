using System.IO;
using UnityEngine;

namespace EE.QC
{
    public class LegFrame
    {
        // Hip, Thigh, Shank
        public Vector3[] positions = new Vector3[3];
        public Quaternion[] rotations = new Quaternion[3];


        public void Update(BinaryReader reader)
        {
            for (int i = 0; i < positions.Length; i++)
                positions[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            for (int i = 0; i < rotations.Length; i++)
                rotations[i] = new Rotation(reader.ReadSingle(), reader.ReadSingle(),
                    reader.ReadSingle(), reader.ReadSingle());
        }
    }
}