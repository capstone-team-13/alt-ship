using EE.AMVCC;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using EasyLayoutNS;
using UnityEditor;
using UnityEngine;

public class HealthUI : Controller<ShipModel>
{
    [SerializeField] private GameObject m_indicatorPrefab;

    private List<GameObject> m_indicators = new();
    private Coroutine m_waveCoroutine;

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
        }
    }

    #endregion

    private void __M_UpdateHealth(int currentHealth)
    {
        var indicatorToGenerate = currentHealth - m_indicators.Count;

        for (var i = 0; i < indicatorToGenerate; ++i)
        {
            var indicator = Instantiate(m_indicatorPrefab, transform);
            m_indicators.Add(indicator);
        }

        if (indicatorToGenerate > 0) Canvas.ForceUpdateCanvases();

        foreach (var indicator in m_indicators) indicator.SetActive(false);
        for (var i = 0; i < currentHealth; ++i) m_indicators[i].SetActive(true);

        __M_ApplyWaveEffect(currentHealth);
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
}