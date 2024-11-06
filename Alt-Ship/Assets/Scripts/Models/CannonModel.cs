using EE.AMVCC;
using UnityEngine;

public class CannonModel : Model
{
    public GameObject Projectile;

    public float InitialSpeed = 5.0f;
    public Vector3 Direction = new(0, 0, 1);

    [Header("Player Interaction")] public float InteractionRadius = 5.0f;

    public LayerMask PlayerLayer;
}