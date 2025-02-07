using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SailMovementSystem : MonoBehaviour
{
    [Header("Ship Variables")]
    public float maxSpeed = 10f;
    public float turnSpeed = 1f;
    public float turnSpeedMax = 6f;
    public float turnSpeedMin = 3f;
    public float sailEfficiency = 1f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    public float sailHeight = 1;

    private bool toggle = false;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (WeatherManager.Instance == null) return;

        Vector3 windDir = WeatherManager.Instance.windDirection.normalized;
        float windIntensity = WeatherManager.Instance.windIntensity;

        float shipAngle = Vector3.Angle(transform.forward, windDir);
        float speedIntensity = WindInput(shipAngle);

        float sailInput = Input.GetAxis("Vertical");

        if (sailInput != 0f) 
        {
            sailHeight = sailHeight + (sailInput * Time.deltaTime);
        }

        sailHeight = Mathf.Clamp(sailHeight, .2f, 1f);

        currentSpeed = maxSpeed * windIntensity * speedIntensity * sailEfficiency * sailHeight;
        rb.velocity = transform.forward * currentSpeed;

        // Temporary for testing
        if (sailHeight == .2f && !toggle)
        {
            turnSpeed = turnSpeedMax;
            toggle = true;
        }
        else if (sailHeight != .2f && toggle)
        {
            turnSpeed = turnSpeedMin;
            toggle = false;
        }

        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, turnInput * turnSpeed * Time.deltaTime);

        //
    }

    private float WindInput(float shipAngle)
    {
        if (shipAngle < 45f) return 1f;
        if (shipAngle > 135f) return 0.2f;
        return Mathf.Lerp(1f,0.2f, (shipAngle - 45f) / 90f);
    }

}
