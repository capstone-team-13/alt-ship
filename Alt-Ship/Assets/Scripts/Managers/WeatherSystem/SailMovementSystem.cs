using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SailMovementSystem : MonoBehaviour
{
    [Header("Ship Variables")]
    public float acceleration = 10f;
    public float turnSpeed = 1f;
    public float turnSpeedMax = 6f;
    public float turnSpeedMin = 3f;
    public float sailEfficiency = 1f;
    public float dragFrontal = .98f;
    public float dragAngular = .95f;
    public float accelerationSmoothing = 2f;
    public float velocitySmoothing = 3f;


    private Rigidbody rb;
    private Vector3 velocity;
    private float angularVelocity;
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

        Movement();
        Drag();
        Tilting();
    }

    private float WindInput(float shipAngle)
    {
        if (shipAngle < 45f) return 1f;
        if (shipAngle > 135f) return 0.2f;
        return Mathf.Lerp(1f,0.2f, (shipAngle - 45f) / 90f);
    }

    private void Movement()
    {
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

        currentSpeed = acceleration * windIntensity * speedIntensity * sailEfficiency * sailHeight;
        float smoothSpeed = Mathf.Lerp(rb.velocity.magnitude, currentSpeed, Time.deltaTime * accelerationSmoothing);
        Vector3 targetVelocity = transform.forward * smoothSpeed;
        Debug.Log("Current Speed: " + smoothSpeed);

        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * velocitySmoothing);


        // Inputs are temporary for testing
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

        if (turnInput != 0f)
        {
            angularVelocity += turnInput * turnSpeed * Time.deltaTime;
        }

        transform.Rotate(Vector3.up, angularVelocity * Time.deltaTime);
    }

    private void Drag()
    {
        velocity *= dragFrontal;
        angularVelocity *= dragAngular;
    }

    private void Tilting()
    {
        float tiltAmount = -angularVelocity * 0.5f;
        transform.localRotation = Quaternion.Euler(0, transform.eulerAngles.y, tiltAmount);
    }

}
