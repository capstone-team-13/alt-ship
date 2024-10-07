using JetBrains.Annotations;
using UnityEngine;

public class ChaoticBeatModule : MonoBehaviour
{
    [SerializeField] private GameObject m_cube1;

    [SerializeField] private GameObject m_cube2;
    // 0 1
    // 00 11 01 10

    [UsedImplicitly]
    private void Start()
    {
        InvokeRepeating(nameof(__M_Version_One), 0.0f, 0.3f);
    }

    // [UsedImplicitly]
    // private void Update()
    // {
    //     __M_Version_One();
    //     // Sleep for a second
    //     float sleepTime;
    // }

    private void __M_Version_One()
    {
        var value = Random.Range(0.0f, 1.0f);
        if (value > 0.5)
        {
            m_cube1.SetActive(false);
            m_cube2.SetActive(true);
        }
        else
        {
            m_cube1.SetActive(true);
            m_cube2.SetActive(false);
        }

        var yValue = Random.Range(0.0f, 1.0f);
        if (value > 0.5 && yValue > 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.y = 1.5f;
            m_cube1.transform.position = newPosition;
        }
        else if (value > 0.5 && yValue < 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.y = 0.5f;
            m_cube1.transform.position = newPosition;
        }
        else if (value < 0.5 && yValue > 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.y = 1.5f;
            m_cube2.transform.position = newPosition;
        }
        else if (value < 0.5 && yValue < 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.y = 0.5f;
            m_cube2.transform.position = newPosition;
        }


        var xValue = Random.Range(0.0f, 1.0f);
        if (value > 0.5 && xValue > 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.x = -2f;
            m_cube1.transform.position = newPosition;
        }
        else if (value > 0.5 && xValue < 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.x = -4f;
            m_cube1.transform.position = newPosition;
        }
        else if (value < 0.5 && xValue > 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.x = 2f;
            m_cube2.transform.position = newPosition;
        }
        else if (value < 0.5 && xValue < 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.x = 4f;
            m_cube2.transform.position = newPosition;
        }

        if (yValue > 0.5 && xValue > 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.x = -2f;
            m_cube1.transform.position = newPosition;
        }
        else if (yValue > 0.5 && xValue < 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.x = -4f;
            m_cube1.transform.position = newPosition;
        }
        else if (yValue < 0.5 && xValue > 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.x = 2f;
            m_cube2.transform.position = newPosition;
        }
        else if (yValue < 0.5 && xValue < 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.x = 4f;
            m_cube2.transform.position = newPosition;
        }

        if (yValue > 0.5 && xValue > 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.x = -1.5f;
            m_cube1.transform.position = newPosition;
        }
        else if (yValue > 0.5 && xValue < 0.5)
        {
            Vector3 newPosition = m_cube1.transform.position;
            newPosition.x = -3.5f;
            m_cube1.transform.position = newPosition;
        }
        else if (yValue < 0.5 && xValue > 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.x = 1.5f;
            m_cube2.transform.position = newPosition;
        }
        else if (yValue < 0.5 && xValue < 0.5)
        {
            Vector3 newPosition = m_cube2.transform.position;
            newPosition.x = 3.5f;
            m_cube2.transform.position = newPosition;
        }

        if (yValue > 0.5 && xValue > 0.5)
        {
            Vector3 newScale = m_cube1.transform.localScale;
            newScale.x = 2f;
            m_cube1.transform.localScale = newScale;
        }
        else if (yValue > 0.5 && xValue < 0.5)
        {
            Vector3 newScale = m_cube1.transform.localScale;
            newScale.x = 1f;
            m_cube1.transform.localScale = newScale;
        }
        else if (yValue < 0.5 && xValue > 0.5)
        {
            Vector3 newScale = m_cube2.transform.localScale;
            newScale.x = 2f;
            m_cube2.transform.localScale = newScale;
        }
        else if (yValue < 0.5 && xValue < 0.5)
        {
            Vector3 newScale = m_cube2.transform.localScale;
            newScale.x = 1f;
            m_cube2.transform.localScale = newScale;
        }

        if (yValue > 0.5 && xValue > 0.5)
        {
            Vector3 newScale = m_cube2.transform.localScale;
            newScale.y = 2f;
            m_cube2.transform.localScale = newScale;
        }
        else if (yValue > 0.5 && xValue < 0.5)
        {
            Vector3 newScale = m_cube2.transform.localScale;
            newScale.y = 1f;
            m_cube2.transform.localScale = newScale;
        }
        else if (yValue < 0.5 && xValue > 0.5)
        {
            Vector3 newScale = m_cube1.transform.localScale;
            newScale.y = 2f;
            m_cube1.transform.localScale = newScale;
        }
        else if (yValue < 0.5 && xValue < 0.5)
        {
            Vector3 newScale = m_cube1.transform.localScale;
            newScale.y = 1f;
            m_cube1.transform.localScale = newScale;
        }
    }
}