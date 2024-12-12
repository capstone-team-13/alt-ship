using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using Application = EE.AMVCC.Application;

public class Goal : MonoBehaviour
{
    [SerializeField] [Space(4)] private UnityEvent OnGameEnd;

    [UsedImplicitly]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            Application.Instance.Push(new GameCommand.GameEnd(Time.time));
            PlayerPrefs.SetInt("Game Result", 1);
            OnGameEnd?.Invoke();
        }
    }
}