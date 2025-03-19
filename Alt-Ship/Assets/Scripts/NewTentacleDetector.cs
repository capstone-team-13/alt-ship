using System.Collections;
using UnityEngine;

public class NewTentacleDetector : MonoBehaviour
{
    [Header("Spawning Type")]
    [SerializeField] private bool preSpawned;
    [SerializeField] private bool rangeSpawned;
    [SerializeField] private bool cyclingSpawns;

    [Header("Spawning Variables")]
    [SerializeField] private float minCycleLength = 5f;
    [SerializeField] private float maxCycleLength = 10f;

    [Header("References")]
    [SerializeField] private GameObject tentacle;
    [SerializeField] private Animator animator;

    private bool shipInRange;
    private bool isUp;
    private bool isAnimating;
    private bool waitingToFall;
    private bool waitingToRise;
    private Coroutine fallCoroutine;
    private float currentCycleTime;

    private void Awake()
    {
        Initialize();

        if (preSpawned && !tentacle.activeSelf && !waitingToRise)
        {
            waitingToRise = true;
            TentacleArise();
        }
    }

    private void Update()
    {
        if (cyclingSpawns)
        {
            if (isUp && !waitingToFall && !shipInRange)
            {
                waitingToFall = true;
                fallCoroutine = StartCoroutine(WaitToFall());
            }

            if (shipInRange && isUp && fallCoroutine != null)
            {
                StopCoroutine(fallCoroutine);
                waitingToFall = false;
            }

            if (!isUp && !waitingToRise && !shipInRange)
            {
                waitingToRise = true;
                StartCoroutine(WaitToRise());
            }
        }
        else
        {
            if (!shipInRange && rangeSpawned && isUp && !waitingToFall)
            {
                waitingToFall = true;
                StartCoroutine(WaitToFall());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Ship") && rangeSpawned && !shipInRange) || (other.CompareTag("Ship") && cyclingSpawns && !shipInRange))
        {
            shipInRange = true;
            TentacleArise();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Ship") && rangeSpawned && shipInRange) || (other.CompareTag("Ship") && cyclingSpawns && shipInRange))
        {
            shipInRange = false;
            if (cyclingSpawns)
            {
                TentacleSubmerge();
            }
            else if (rangeSpawned)
            {
                TentacleSubmerge();
            }
        }
    }

    private void Initialize()
    {
        isUp = false;
        shipInRange = false;
        isAnimating = false;
        waitingToFall = false;
        waitingToRise = false;

        currentCycleTime = Random.Range(minCycleLength, maxCycleLength);
    }

    private void TentacleArise()
    {
        if (!isUp && !isAnimating)
        {
            StartCoroutine(TentRise());
        }
    }

    IEnumerator TentRise()
    {
        tentacle.SetActive(true);
        isAnimating = true;
        animator.SetBool("isUp", true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isUp = true;
        isAnimating = false;
        waitingToRise = false;
    }

    private void TentacleSubmerge()
    {
        if (isUp && !isAnimating)
        {
            StartCoroutine(TentFall());
        }
    }

    IEnumerator WaitToFall()
    {
        yield return new WaitForSeconds(currentCycleTime);

        if (isUp && !isAnimating && tentacle.activeSelf)
        {
            waitingToFall = false;
            TentacleSubmerge();
        }
    }

    IEnumerator WaitToRise()
    {
        yield return new WaitForSeconds(currentCycleTime);

        if (!isAnimating && !tentacle.activeSelf)
        {
            TentacleArise();
        }
    }

    IEnumerator TentFall()
    {
        isAnimating = true;
        animator.SetBool("isUp", false);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isUp = false;
        isAnimating = false;
        waitingToFall = false;
        tentacle.SetActive(false);

        currentCycleTime = Random.Range(minCycleLength, maxCycleLength); // Set new random cycle time after falling
    }
}
