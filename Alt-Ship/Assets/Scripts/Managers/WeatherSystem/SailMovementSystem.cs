using System.Collections;
using System.Collections.Generic;
using EE.AMVCC;
using Application = EE.AMVCC.Application;
using JetBrains.Annotations;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SailMovementSystem : Controller<ShipModel>
{
    [Header("Ship Variables")] public float acceleration = 10f;
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

    private float previousSpeed;
    private bool toggle = false;

    [Header("Tilting")] public bool tiltingToggle = true;
    public float tiltAmount = 2f;
    public float tiltSpeed = .5f;
    public float forwardTilt = 1f;
    public float forwardTiltSpeed = .3f;

    [Header("Sheep")] public GameObject sheepOne;
    public GameObject sheepTwo;
    public GameObject sheepThree;
    public float mincollisionSpeed = 8f;
    public float collisionCD = 10f;
    private bool collisionToggle = false;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (tiltingToggle)
        {
            float sideTilt = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
            float frontTilt = Mathf.Sin(Time.time * forwardTiltSpeed) * forwardTilt;

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

        previousSpeed = rb.velocity.magnitude;
    }

    private float WindInput(float shipAngle)
    {
        if (shipAngle < 45f) return 1f;
        if (shipAngle > 135f) return 0.2f;
        return Mathf.Lerp(1f, 0.2f, (shipAngle - 45f) / 90f);
    }

    private void Movement()
    {
        Vector3 windDir = WeatherManager.Instance.windDirection.normalized;
        float windIntensity = WeatherManager.Instance.windIntensity;

        float shipAngle = Vector3.Angle(transform.forward, windDir);
        float speedIntensity = WindInput(shipAngle);

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

        this.gameObject.transform.Rotate(Vector3.up, angularVelocity * Time.deltaTime);
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
            case ShipCommand.Damage damageCommand:
                Model.Health -= damageCommand.Value;
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
            Debug.Log("No Input, Slowing Down");
            angularVelocity = Mathf.Lerp(angularVelocity, 0f, Time.deltaTime * 2f);
        }
    }

    private void Drag()
    {
        velocity *= dragFrontal;
        rb.angularVelocity *= dragAngular;
        angularVelocity = Mathf.Lerp(angularVelocity, angularVelocity * 0.8f, Time.deltaTime * 1.5f);
    }

    private void Tilting()
    {
        float tiltAmount = -angularVelocity * 0.5f;
        transform.localRotation = Quaternion.Euler(0, transform.eulerAngles.y, tiltAmount);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Collision] Collide with {collision.gameObject.tag}");
        if (collisionToggle) return;
        Debug.Log("[Collision] Not in cooling down");

        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("[Collision] Damage Command Shoot");
            Application.Instance.Push(new ShipCommand.Damage(1));
            StartCoroutine(SheepCooldown(collisionCD));
        }

        float currentSpeed = rb.velocity.magnitude;
        float speedDrop = previousSpeed - currentSpeed;
        if (speedDrop > mincollisionSpeed && collision.gameObject.tag == "Obstacle")
        {
            rb.velocity *= .5f;
            if (sheepOne.activeSelf)
            {
                SheepFling(sheepOne);
                return;
            }

            if (sheepTwo.activeSelf)
            {
                SheepFling(sheepTwo);
                return;
            }

            if (sheepThree.activeSelf)
            {
                SheepFling(sheepThree);
                return;
            }
        }

    }

    private void SheepFling(GameObject sheep)
    {
        if (sheep == null) return;

        sheep.transform.parent = null;

        Vector3 flingDirection = (transform.up + transform.forward).normalized * 5f;

        StartCoroutine(FlingMovement(sheep, flingDirection, 5f));
    }

    private IEnumerator FlingMovement(GameObject sheep, Vector3 flingDirection, float duration)
    {
        sheep.transform.parent = null;
        float timeElapsed = 0f;

        Vector3 startPosition = sheep.transform.position;
        Vector3 targetPosition = startPosition + flingDirection * 5f;

        Quaternion startRotation = sheep.transform.rotation;

        Quaternion targetRotation = startRotation * Quaternion.Euler(Random.insideUnitSphere * 360f);

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            sheep.transform.position = Vector3.Lerp(startPosition, targetPosition, t * 10f);
            sheep.transform.rotation = Quaternion.RotateTowards(startRotation, targetRotation, t * 360f);
            targetPosition.y = Mathf.Lerp(targetPosition.y, 0f, t); // 0f is water surface height

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        sheep.transform.position = new Vector3(sheep.transform.position.x, 0f, sheep.transform.position.z);

        sheep.SetActive(false);
    }

    private IEnumerator SheepCooldown(float cooldown)
    {
        collisionToggle = true;
        yield return new WaitForSeconds(cooldown);
        collisionToggle = false;
    }
}