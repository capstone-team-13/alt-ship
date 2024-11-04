using UnityEngine;
using Application = EE.AMVCC.Application;

public class DeathZone : MonoBehaviour
{
    [SerializeField]
    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag("Player")) return;

        var playerModel = other.gameObject.GetComponent<PlayerModel>();
        if (playerModel == null)
            throw new System.NullReferenceException($"Need attach {nameof(PlayerModel)} to the object");
        Application.Instance.Push(new PlayerCommand.Dead(playerModel));
    }
}