using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour, IController
{
    public void Notify<TCommand>(TCommand command) where TCommand : ICommand
    {
        switch (command)
        {
            case PlayerCommand.Dead deadCommand:
                var player = deadCommand.Player.gameObject;
                player.transform.SetParent(null);
                Debug.Log($"{player.name} dead.");
                break;
        }
    }
}