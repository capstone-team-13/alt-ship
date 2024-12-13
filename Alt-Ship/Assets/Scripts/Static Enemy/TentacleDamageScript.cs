using UnityEngine;

public class HitboxDetection : MonoBehaviour
{
    public TentacleDamageDetection parentScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CannonBall"))
        {
            parentScript.HandleCannonBallHit(other);
        }
    }
}