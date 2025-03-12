using DG.Tweening;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class GoalIndicatorUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] m_hints;
    [SerializeField] private float m_scaleDuration = 0.5f;
    [SerializeField] private float m_showDuration = 2f;
    [SerializeField] private float m_fadeDuration = 1f;

    private int m_currentHintIndex = 0;

    [UsedImplicitly]
    public void ShowHint(string objectName)
    {
        if (!objectName.Contains("Text")) return;

        var previousIndex = Mathf.Max(m_currentHintIndex - 1, 0);
        m_hints[previousIndex].gameObject.SetActive(false);

        var currentHint = m_hints[m_currentHintIndex];
        currentHint.gameObject.SetActive(true);
        currentHint.transform.localScale = Vector3.one * 2f;
        currentHint.transform.DOScale(Vector3.one, m_scaleDuration).SetEase(Ease.OutQuad)
            .OnComplete(() => StartCoroutine(FadeOutHint(currentHint)));

        m_currentHintIndex = (m_currentHintIndex + 1) % m_hints.Length;
    }

    private IEnumerator FadeOutHint(CanvasGroup hint)
    {
        yield return new WaitForSeconds(m_showDuration);

        hint.DOFade(0, m_fadeDuration).SetEase(Ease.OutQuad)
            .OnComplete(() => hint.gameObject.SetActive(false));
    }
}