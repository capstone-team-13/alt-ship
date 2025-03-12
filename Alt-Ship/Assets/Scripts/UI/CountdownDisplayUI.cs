using DG.Tweening;
using EE.AMVCC;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Application = EE.AMVCC.Application;

public class CountdownDisplayUI : MonoBehaviour, IController
{
    [SerializeField] private CountDown m_counter;
    [SerializeField] private TMP_Text m_countdownText;

    [SerializeField] private CanvasGroup m_canvasGroup;

    [UsedImplicitly]
    private void OnEnable()
    {
        m_counter.onCountdownUpdated.AddListener(UpdateRemainingTime);
        m_counter.onCountdownFinished.AddListener(EndGame);
    }

    [UsedImplicitly]
    private void OnDisable()
    {
        m_counter.onCountdownUpdated.RemoveListener(UpdateRemainingTime);
        m_counter.onCountdownFinished.RemoveListener(EndGame);
    }

    public void EndGame()
    {
        Application.Instance.Push(new GameCommand.GameEnd(Time.time));
        PlayerPrefs.SetInt("Game Result", (int)GameResult.Lose);
    }

    public void UpdateRemainingTime(float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        m_countdownText.text = $"{minutes:00}:{seconds:00}";

        if (remainingTime < 60)
        {
            m_countdownText.color = Color.red;
            Vector3 originalPosition = m_countdownText.transform.localPosition;
            m_countdownText.transform.localPosition =
                originalPosition + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);
        }
        else
        {
            m_countdownText.color = new Color(1f, 0.796f, 0.039f);
            m_countdownText.transform.localPosition = new Vector3(0, m_countdownText.transform.localPosition.y,
                m_countdownText.transform.localPosition.z);
        }
    }

    [UsedImplicitly]
    public void FadeIn(string objectName)
    {
        if (!objectName.Contains("ShowTimer")) return;

        var canvasGroupGameObject = m_canvasGroup.gameObject;

        m_canvasGroup.alpha = 0;
        canvasGroupGameObject.transform.localScale = Vector3.zero;
        canvasGroupGameObject.SetActive(true);
        var sequence = DOTween.Sequence();

        sequence.Append(m_canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad))
            .Join(canvasGroupGameObject.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack))
            .OnStart(() => { UpdateRemainingTime(m_counter.GetTime()); });
    }

    public void Notify<TCommand>(TCommand command) where TCommand : ICommand
    {
        switch (command)
        {
            case GameCommand.GameStart:
                m_counter.StartCountdown();
                break;
        }
    }
}