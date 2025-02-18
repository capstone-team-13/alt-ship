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
        Debug.Log("Goal Reached");
        if (collision.gameObject.CompareTag("ShipParent"))
        {
            Application.Instance.Push(new GameCommand.GameEnd(Time.time));
            PlayerPrefs.SetInt("Game Result", (int)GameResult.Win);
            OnGameEnd?.Invoke();
        }
    }
}