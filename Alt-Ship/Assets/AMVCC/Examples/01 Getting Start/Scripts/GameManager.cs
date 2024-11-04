using JetBrains.Annotations;
using UnityEngine;

namespace EE.AMVCC.Examples
{
    public class GameManager : MonoBehaviour
    {
        [UsedImplicitly]
        private void Start()
        {
            Application.Instance.Push(new BallCommand.LogCommand("I'm a ball"));
        }

    }
}