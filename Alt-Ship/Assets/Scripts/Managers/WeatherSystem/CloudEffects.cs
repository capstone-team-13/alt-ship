using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffects : MonoBehaviour
{
    [Header("Movement Variations")]
    public float directionRate = .05f;
    public float speedScale = .01f;
    public float speedIntensity = .3f;

    private Vector3 targetSpeed;
    private float spawnTime;
    private float lifetime;
    private float transparency = 1f;

    public event Action OnCloudDespawn;

    private void Start()
    {
        spawnTime = Time.time;
        lifetime = UnityEngine.Random.Range(30f,60f);
    }

    void Update()
    {
        if (WeatherManager.Instance == null) return;

        Drift();

        if(Time.time - spawnTime > lifetime - 15) 
        {
            if (transform.GetComponentInChildren<Renderer>() != null)
            {
                Renderer renderer = transform.GetComponentInChildren<Renderer>();
                transparency = Mathf.Lerp(transparency, 0, (lifetime - (Time.time - spawnTime)) * Time.deltaTime);
                renderer.material.SetColor("_Color", new Color(1, 1, 1, transparency));
            }

        }

        if(Time.time - spawnTime > lifetime)
        {
            OnCloudDespawn?.Invoke();
            Destroy(gameObject);
        }
    }

    private void Drift()
    {
        Quaternion targetDirection = Quaternion.LookRotation(WeatherManager.Instance.windDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetDirection, Time.deltaTime * directionRate);

        float trueSpeed = speedIntensity + (transform.position.y * speedScale);
        targetSpeed = WeatherManager.Instance.windDirection * WeatherManager.Instance.windIntensity;
        transform.position += targetSpeed * trueSpeed * Time.deltaTime; 
    }


}
