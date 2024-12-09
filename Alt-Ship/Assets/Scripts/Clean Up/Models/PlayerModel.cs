using EE.AMVCC;
using UnityEngine;

namespace EE.CU
{
    public class PlayerModel : Model
    {
        [Header("Survival")] public int Health = 5;
        public bool Dead = false;

        [Header("Movement")]
        // If speed <= 0, player cannot move
        public float Speed = 5;

        public Vector3 Direction;

        public Vector3 Velocity => Speed * Direction;
    }
}