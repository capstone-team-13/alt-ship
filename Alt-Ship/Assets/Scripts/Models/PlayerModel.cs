using EE.AMVCC;
using UnityEngine;

public class PlayerModel : Model
{
    public int Health = 5;
    public bool Dead = false;

    public float Speed = 5;
    public Vector3 Direction;

    public Vector3 Velocity => Speed * Direction;
}