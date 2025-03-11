using UnityEngine;
using UnityEngine.Events;

public class CountDown : MonoBehaviour
{
    [SerializeField] private float m_time;

    [Space(4)] public UnityEvent<float> onCountdownUpdated;
    [Space(4)] public UnityEvent onCountdownFinished;

    private float m_remainingTime;
    private bool m_isCountingDown;

    private void Update()
    {
        if (!m_isCountingDown || !(m_remainingTime > 0)) return;
        m_remainingTime -= Time.deltaTime;

        onCountdownUpdated?.Invoke(m_remainingTime);

        if (!(m_remainingTime <= 0)) return;

        m_remainingTime = 0;
        m_isCountingDown = false;
        onCountdownFinished?.Invoke();
    }

    public void StartCountdown()
    {
        m_remainingTime = m_time;
        m_isCountingDown = true;
    }

    public void StopCountdown()
    {
        m_isCountingDown = false;
    }

    public float GetRemainingTime()
    {
        return m_remainingTime;
    }
}