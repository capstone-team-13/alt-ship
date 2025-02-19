using System.Collections;
using System.Collections.Generic;
using EE.AMVCC;
using JetBrains.Annotations;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SailMovementSystem : Controller<ShipModel>
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

    [Header("Tilting")]
    public bool tiltingToggle = true;
    public float tiltAmount = 2f;
    public float tiltSpeed = .5f;
    public float forwardTilt = 1f;
    public float forwardTiltSpeed = .3f;
    public float torque = .1f;



    private float timeOffset;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        timeOffset = .1f;
    }

    private void Update()
    {
        if (tiltingToggle)
        {
            float sideTilt = Mathf.Sin(Time.time * tiltSpeed + timeOffset) * tiltAmount;
            float frontTilt = Mathf.Sin(Time.time * forwardTiltSpeed + timeOffset) * forwardTilt;

            transform.localRotation = Quaternion.Euler(frontTilt, transform.eulerAngles.y, sideTilt);
        }
    }

    private void FixedUpdate()
    {
        if (WeatherManager.Instance == null) return;
        sailHeight = Model.Speed;
        Movement();
        Drag();
        Tilting();

        if (tiltingToggle)
        {
            float force = Mathf.Sin(Time.time * tiltSpeed + timeOffset) * torque;
            rb.AddTorque(Vector3.up * force, ForceMode.Acceleration);
        }
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
        //Debug.Log("Current Speed: " + smoothSpeed);

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

        transform.Rotate(Vector3.up, angularVelocity * Time.deltaTime);

    }

    public override void Notify<TCommand>(TCommand command)
    {
        if (command is not ShipCommand) return;

        switch (command)
        {
            case ShipCommand.Steer steerCommand:
                var sign = steerCommand.RotationSign;
                __M_Steer(sign);
                break;
        }
    }

    private void __M_Steer(float sign)
    {
        if (sign != 0f)
        {
            angularVelocity += sign * turnSpeed * Time.deltaTime;
            angularVelocity = Mathf.Clamp(angularVelocity, -turnSpeedMax, turnSpeedMax);
        }
        else
        {
            angularVelocity = Mathf.Lerp(angularVelocity, 0f, Time.deltaTime * 2f);
        }
    }

    private void Drag()
    {
        velocity *= dragFrontal;
        rb.angularVelocity *= dragAngular;
        angularVelocity = Mathf.Lerp(angularVelocity, 0f, Time.deltaTime * 1.5f);
    }

    private void Tilting()
    {
        float tiltAmount = -angularVelocity * 0.5f;
        transform.localRotation = Quaternion.Euler(0, transform.eulerAngles.y, tiltAmount);
    }

}
