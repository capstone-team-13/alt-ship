using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;
using static GameCommand;
using Application = EE.AMVCC.Application;

public class GameManager : MonoBehaviour, IController
{
    [UsedImplicitly]
    private void Start()
    {
        Debug.Log($"Game starts @ {Time.time}");
        Application.Instance.Push(new GameStart(Time.time));
    }

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