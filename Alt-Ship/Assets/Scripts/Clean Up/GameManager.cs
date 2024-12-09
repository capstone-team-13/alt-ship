using EE.AMVCC;
using JetBrains.Annotations;
using UnityEngine;
using Application = EE.AMVCC.Application;

namespace EE.CU
{
    using static GameCommand;

    public class GameManager : MonoBehaviour, IController
    {
        [UsedImplicitly]
        private void Start()
        {
            var startTime = Time.timeSinceLevelLoad;
            Debug.Log($"Game starts @ {startTime}");
            Application.Instance.Push(new GameStart(startTime));
        }

        public void Notify<TCommand>(TCommand command) where TCommand : ICommand
        {
            switch (command)
            {
                case PlayerCommand.Dead deadCommand:
                    GameObject player = deadCommand.Player.gameObject;
                    player.transform.SetParent(null);
                    Debug.Log($"{player.name} dead.");
                    break;
            }
        }
    }
}