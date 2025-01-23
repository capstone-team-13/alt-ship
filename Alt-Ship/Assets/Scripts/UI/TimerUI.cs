using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_remainingTimeText;

    private Coroutine m_currentCountdownCoroutine;

    public void StartCountDown(float timeInSeconds)
    {
        if (m_currentCountdownCoroutine != null)
        {
            StopCoroutine(m_currentCountdownCoroutine);
            m_currentCountdownCoroutine = null;
        }

        m_currentCountdownCoroutine = StartCoroutine(CountDownCoroutine(timeInSeconds));
    }

    public void StopCountDown()
    {
        if (m_currentCountdownCoroutine == null) return;
        StopCoroutine(m_currentCountdownCoroutine);
        m_currentCountdownCoroutine = null;
        m_remainingTimeText.text = "";
    }

    private IEnumerator CountDownCoroutine(float timeInSeconds)
    {
        float remainingTime = timeInSeconds;

        while (remainingTime > 0)
        {
            UpdateRemainingTimeText(remainingTime);

            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        UpdateRemainingTimeText(0);
        m_currentCountdownCoroutine = null;
    }

    private void UpdateRemainingTimeText(float time)
    {
        m_remainingTimeText.text = Mathf.CeilToInt(time).ToString();

        Vector3 originalScale = m_remainingTimeText.transform.localScale;

        m_remainingTimeText.transform
            .DOScale(originalScale * 1.2f, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                m_remainingTimeText.transform
                    .DOScale(originalScale, 0.2f)
                    .SetEase(Ease.InQuad);
            });
    }
}