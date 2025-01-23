using TMPro;
using UnityEngine;
using Application = EE.AMVCC.Application;

public class CountdownDisplayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_countdownText;

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
}