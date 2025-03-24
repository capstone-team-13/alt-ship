using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class NewTentacleSystem : MonoBehaviour
{
    [Header("Mode (Enable One)")]
    // Will have a bar telling the player how close they are to being attacked.
    [SerializeField] private bool instant;
    // Will have a bar telling the player how soon they will be attacked while in range.
    [SerializeField] private bool overtime;
    // Will have a bar overtime, and will attack if the player gets too close.
    [SerializeField] private bool overtimeInstant;

    [Header("Parameters")]
    // Distance the tentacle will attack at. (Instant / OvertimeInstant)
    [SerializeField] private float minDistance;
    // Distance the tentacle will start tracing the players distance (Instant)
    [SerializeField] private float outerDistanceInstant;
    // Distance the tentacle will start trying to attack the player (Overtime / OvertimeInstant)
    [SerializeField] private float distanceOvertime;
    // Time it takes the tentacle to attack the player (Overtime / OvertimeInstant)
    [SerializeField] private float totaltimeOvertime;

    [Header("References (Don't Touch)")]
    [SerializeField] private NewTentacleDetector newTentacleDetector;
    [SerializeField] private GameObject shipParent;
    [SerializeField] private GameObject tentacle;
    public Slider slider;

    private BarTrigger barTrigger;
    private GameObject sliderParent;
    private bool sliderInitialized;

    private bool inRangeCheck;

    public bool disableFunction = false;

    private float timer = 0f;

    private void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("ShipParent");
        if (obj != null)
        {
            shipParent = obj;
        }

        GameObject sldr = GameObject.FindGameObjectWithTag("TentacleSlider");
        if(sldr != null)
        {
            sliderParent = sldr;
            barTrigger = sldr.GetComponent<BarTrigger>();
           // slider = sldr.GetComponentInChildren<Slider>();
        }

        tentacle = newTentacleDetector.tentacle;
        inRangeCheck = true;
        sliderInitialized = false;
    }

    private void Update()
    {
        if (disableFunction) return;

        if (newTentacleDetector.shipInRange)
        {
            LookAtShip();
        }

        //

        if (instant && newTentacleDetector.shipInRange)
        {
            InstantRange();
        }
        else if (overtime && newTentacleDetector.shipInRange)
        {
            OverTimeCheck();
        }
        else if (overtimeInstant && newTentacleDetector.shipInRange)
        {
            OverTimeInstantCheck();
        }


        //

        if(inRangeCheck != newTentacleDetector.shipInRange)
        {
            DisableSlider();
        }

        inRangeCheck = newTentacleDetector.shipInRange;
    }


    private void OnDrawGizmos()
    {
        if (tentacle != null) 
        {

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tentacle.transform.position, minDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tentacle.transform.position, outerDistanceInstant);

        }
    }


    private void LookAtShip()
    {
        if (shipParent != null && tentacle.activeSelf)
        {
            Vector3 direction = (shipParent.transform.position - tentacle.transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

            tentacle.transform.rotation = Quaternion.Lerp(tentacle.transform.rotation, targetRotation, Time.deltaTime * 3f);
        }
    }

    private void InstantRange()
    {
        float squaredDistance = (tentacle.transform.position - shipParent.transform.position).sqrMagnitude;
        float outerRange = outerDistanceInstant * outerDistanceInstant;
        float innerRange = minDistance * minDistance;

        if (squaredDistance >= innerRange && squaredDistance <= outerRange) 
        {
            if (barTrigger != null && slider == null && !sliderInitialized)
            {
                EnableSlider();
            }
            else if (slider != null && !slider.gameObject.activeSelf)
            {
                slider.gameObject.SetActive(true);
            }

            float distance = Mathf.Sqrt(squaredDistance);

            // % The ship is to the center
            float progress = Mathf.Clamp01( 1 - ((distance - minDistance) / (outerDistanceInstant - minDistance)));

            if (slider != null)
            {
                slider.maxValue = 1;
                slider.minValue = 0;
                slider.value = progress;
            }

        }
        else if (squaredDistance < innerRange)
        {
            DisableSlider();
            DisableSystems();
            newTentacleDetector.TentacleAttackStart();
        }
        else
        {
            // Ship out of range
        }
    }

    private void OverTimeCheck()
    {
        float squaredDistance = (tentacle.transform.position - shipParent.transform.position).sqrMagnitude;
        float outerRange = distanceOvertime * distanceOvertime;

        if (squaredDistance <= outerRange && timer <= totaltimeOvertime)
        {
            if (barTrigger != null && slider == null && !sliderInitialized)
            {
                EnableSlider();
                timer = 0f;
            }
            else if (slider != null && !slider.gameObject.activeSelf)
            {
                slider.gameObject.SetActive(true);
                timer = 0f;
            }

            timer += Time.deltaTime;

            float percentage = Mathf.Clamp01(timer / totaltimeOvertime);

            if (slider != null && timer <= totaltimeOvertime)
            {
                slider.value = percentage;
            }
        }
        else if (timer >= totaltimeOvertime)
        {
            DisableSlider();
            DisableSystems();
            newTentacleDetector.TentacleAttackStart();
            //Start attack
        }
        else { }
    }

    private void OverTimeInstantCheck()
    {
        float squaredDistance = (tentacle.transform.position - shipParent.transform.position).sqrMagnitude;
        float outerRange = outerDistanceInstant * outerDistanceInstant;
        float innerRange = minDistance * minDistance;

        if (squaredDistance >= innerRange && squaredDistance <= outerRange && timer <= totaltimeOvertime)
        {
            if (barTrigger != null && slider == null && !sliderInitialized)
            {
                EnableSlider();
                timer = 0f;
            }
            else if (slider != null && !slider.gameObject.activeSelf)
            {
                slider.gameObject.SetActive(true);
                timer = 0f;
            }

            timer += Time.deltaTime;

            float percentage = Mathf.Clamp01(timer / totaltimeOvertime);

            if (slider != null && timer <= totaltimeOvertime)
            {
                slider.value = percentage;
            }

        }
        else if (squaredDistance < innerRange)
        {
            DisableSlider();
            DisableSystems();
            newTentacleDetector.TentacleAttackStart();

            // Attack
        }
        else if(timer >= totaltimeOvertime)
        {
            DisableSlider();
            DisableSystems();
            newTentacleDetector.TentacleAttackStart();

            // Attack
        }
        else
        {
            // Ship out of range
        }
    }
    

    private void DisableSlider()
    {
        if (slider != null && slider.gameObject.activeSelf)
        {
            slider.gameObject.SetActive(false);
        }
    }

    private void EnableSlider()
    {
        barTrigger.toggleSlider();
        slider = sliderParent.GetComponentInChildren<Slider>();
        sliderInitialized = true;
    }
    
    private void DisableSystems()
    {
        disableFunction = true;
    }

}
