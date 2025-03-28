using DG.Tweening;
using EE.AMVCC;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Application = EE.AMVCC.Application;
using CanvasGroup = UnityEngine.CanvasGroup;

public class HealthUI : Controller<ShipModel>
{
    [SerializeField] private GameObject m_indicatorPrefab;

    // TODO: TEMP
    [SerializeField] private SceneLoader m_sceneLoader;

    private List<GameObject> m_indicators = new();
    private Coroutine m_waveCoroutine;

    [Header("Refs.")] [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private RectTransform m_bouncySheep;

    [UsedImplicitly]
    private void Start()
    {
        __M_UpdateHealth(Model.Health);
        InvokeRepeating(nameof(__M_ApplyWaveEffect), 0.0f, 8.0f);
    }

    #region API

    public override void Notify<TCommand>(TCommand command)
    {
        if (command is not ShipCommand) return;

        switch (command)
        {
            case ShipCommand.HealthUpdate healthUpdateCommand:
                __M_UpdateHealth(healthUpdateCommand.CurrentHealth);
                break;
            case ShipCommand.Damage damageCommand:
                Model.Health -= damageCommand.Value;
                __M_UpdateHealth(Model.Health);
                break;
        }
    }

    [UsedImplicitly]
    public void FadeIn(string objectName)
    {
        if (!objectName.Contains("ShowHealth")) return;

        var canvasGroupGameObject = m_canvasGroup.gameObject;

        m_canvasGroup.alpha = 0;
        canvasGroupGameObject.transform.localScale = Vector3.zero;
        canvasGroupGameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        sequence.Append(m_canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad))
            .Join(canvasGroupGameObject.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack));
    }

    #endregion

    private void __M_UpdateHealth(int currentHealth)
    {
        Debug.Log("Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Current Health Two: " + currentHealth);
            Application.Instance.Push(new GameCommand.GameEnd(Time.time));
            PlayerPrefs.SetInt("Game Result", (int)GameResult.Lose);
            m_sceneLoader.Load();
            return;
        }

        var indicatorToGenerate = currentHealth - m_indicators.Count;

        for (var i = 0; i < indicatorToGenerate; ++i)
        {
            var indicator = Instantiate(m_indicatorPrefab, transform);
            m_indicators.Add(indicator);
        }

        foreach (var indicator in m_indicators) indicator.SetActive(false);
        for (var i = 0; i < currentHealth; ++i) m_indicators[i].SetActive(true);

        // Temp checking if player is taking damage
        // if (currentHealth < m_indicators.Count) __M_BounceSheep();

        Canvas.ForceUpdateCanvases();

        __M_ApplyWaveEffect(currentHealth);
        Debug.Log("Current Health: " + currentHealth);

    }

    private void __M_ApplyWaveEffect()
    {
        if (m_waveCoroutine != null) StopCoroutine(m_waveCoroutine);

        m_waveCoroutine = StartCoroutine(__M_WaveEffect(Model.Health));
    }

    private void __M_ApplyWaveEffect(int activeCount)
    {
        if (m_waveCoroutine != null) StopCoroutine(m_waveCoroutine);

        m_waveCoroutine = StartCoroutine(__M_WaveEffect(activeCount));
    }

    private IEnumerator __M_WaveEffect(int activeCount)
    {
        const float duration = 0.5f;
        const float amplitude = 10f;

        for (var i = 0; i < activeCount; ++i)
        {
            var indicator = m_indicators[i];
            var originalPosition = indicator.transform.localPosition;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                var offset = Mathf.Sin((elapsedTime / duration) * Mathf.PI) * amplitude;
                indicator.transform.localPosition = originalPosition + new Vector3(0, offset, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            indicator.transform.localPosition = originalPosition;
        }
    }

    private void __M_BounceSheep()
    {
        m_bouncySheep.anchoredPosition = new Vector2(0, 32);
        m_bouncySheep.gameObject.SetActive(true);
        m_bouncySheep.localScale = Vector3.one;

        StartCoroutine(__M_StartBounceAndShrink());
    }

    private IEnumerator __M_StartBounceAndShrink()
    {
        const float duration = 3f;
        const float initialUpForce = 2500f;

        var canvas = m_bouncySheep.GetComponentInParent<Canvas>();
        var canvasRect = canvas.GetComponent<RectTransform>();

        var screenBounds = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height);

        var elapsedTime = 0f;

        float randomXForce = Random.Range(-initialUpForce, initialUpForce);
        var velocity = new Vector2(randomXForce, initialUpForce);

        m_bouncySheep.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .SetDelay(duration)
            .OnComplete(() => m_bouncySheep.gameObject.SetActive(false));

        while (elapsedTime < duration + 0.5f)
        {
            var newPosition = m_bouncySheep.anchoredPosition + (velocity * Time.deltaTime);

            if (Mathf.Abs(newPosition.x) > screenBounds.x)
            {
                velocity.x *= -1;
                newPosition.x = Mathf.Sign(newPosition.x) * screenBounds.x;
                m_bouncySheep.localScale *= 0.99f;
            }

            if (newPosition.y <= 0 || newPosition.y >= screenBounds.y)
            {
                velocity.y *= -1;
                m_bouncySheep.localScale *= 0.99f;
            }

            m_bouncySheep.anchoredPosition = newPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}