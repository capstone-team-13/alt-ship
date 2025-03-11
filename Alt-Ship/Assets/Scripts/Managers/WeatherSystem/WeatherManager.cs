using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance {  get; private set; }

    [Header("Global Wind Settings")]
    public Vector3 windDirection = Vector3.forward;
    public float windIntensity = 3f;

    [Header("Wind Movement")]
    public float windDirectionRate = .05f;
    public float windIntensityRate = 1f;
    public float maxIntensity = 6f;
    public float minIntensity = 2.5f;
    public float windChangeTime = 10f;

    private float initialIntensity;
    private Vector3 targetDirection;
    private float targetIntensity;
    private Coroutine windChange;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        initialIntensity = 0f;
        targetIntensity = minIntensity;

        windDirection = Vector3.forward; 

        Invoke(nameof(WindVariations), windChangeTime);
        InvokeRepeating(nameof(WindVariations), windChangeTime, windChangeTime);
    }

    private void Update()
    {
        windDirection = Vector3.Slerp(windDirection, targetDirection, Time.deltaTime * windDirectionRate);
        windIntensity = Mathf.Lerp(windIntensity, targetIntensity, Time.deltaTime * windIntensityRate);
    }

    private void WindVariations()
    {
        Vector3 referenceForward = Vector3.forward;

        float angle = Random.Range(-90f, 90f);

        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);

        targetDirection = rotation * referenceForward;

        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    public void WindOverride(Vector3 newDirection, float newIntensity, float transitionSpeed)
    {
        if (windChange != null)
            StopCoroutine(windChange);

        windChange = StartCoroutine(WindTransition(newDirection, newIntensity, transitionSpeed));
    }

    private IEnumerator WindTransition(Vector3 newDirection, float newIntensity, float transitionSpeed)
    {
        float t = 0f;
        Vector3 initialDirection = windDirection;
        initialIntensity = windIntensity;

        while(t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            windDirection = Vector3.Slerp(initialDirection, newDirection, t);
            windIntensity = Mathf.Lerp(initialIntensity, newIntensity, t);
            yield return null;
        }
        windDirection = newDirection;
        windIntensity = newIntensity;
    }


}
