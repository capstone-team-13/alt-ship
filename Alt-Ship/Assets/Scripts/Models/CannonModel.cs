using EE.AMVCC;
using UnityEngine;

public class CannonModel : Model
{
    public Rigidbody Projectile;
    public Transform ShootAt;

    public float InitialForce = 1.0f;
}