using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WindEffectManager : MonoBehaviour
{
    private ParticleSystem particleSystem;
    public float rotSpeed = 2f;

    private bool toggle = true;

    private float targetLength = 0f;
    private float lengthRate = .5f;

    void Start()
    {
        particleSystem = this.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(particleSystem != null)
        {
            changeWind();
        }

        var trailLife = particleSystem.trails;
        trailLife.enabled = true;
        trailLife.lifetime = Mathf.Lerp(trailLife.lifetime.constant,targetLength, lengthRate * Time.deltaTime);

    }

    private void changeWind()
    {
        if (WeatherManager.Instance == null) return;
 
        var spawnRate = particleSystem.emission;
        spawnRate.enabled = true;
        var vel = particleSystem.velocityOverLifetime;
        vel.speedModifier = WeatherManager.Instance.windIntensity;
        Vector3 windDirection = WeatherManager.Instance.windDirection;

        float midIntensity = (WeatherManager.Instance.maxIntensity + WeatherManager.Instance.minIntensity)/2;
        if (WeatherManager.Instance.windIntensity >= midIntensity && !toggle)
        {
            Debug.Log("Wind is fast");
            targetLength = .15f;
            spawnRate.rateOverTime = .5f;
            toggle = true;
        }
        else if(WeatherManager.Instance.windIntensity < midIntensity && toggle)
        {
            Debug.Log("Wind is slow");
            targetLength = .075f;
            spawnRate.rateOverTime = 1f;
            toggle = false;
        }

        if(windDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(windDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        }
    }
}
