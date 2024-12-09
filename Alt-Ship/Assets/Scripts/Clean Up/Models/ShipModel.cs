using EE.AMVCC;
using UnityEngine;

namespace EE.CU
{
    public class ShipModel : Model
    {
        [Header("Survival")] public int Health = 5;

        [Header("Movement")] public float Speed;

        public Vector3 Direction = new(0, 0, 1);
        public Vector3 Velocity => Speed * Direction;

        [Header("Steer")] public Vector3 SailDirection;
        public float RotationSpeed = 5f;
        public float LerpSpeed = 5f;
        public Vector3 TargetEulerAngles;
    }
}