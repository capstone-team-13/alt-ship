using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NewTentacleDetector : MonoBehaviour
{
    [Header("Spawning Type")]
    // Tentacles will all always be spawned when the game starts
    [SerializeField] private bool preSpawned;
    // Tentacles will only spawn in when the player is in range, and will go back down when they are not
    [SerializeField] private bool rangeSpawned;
    // Tentacles will go up and down periodically, and will always stay up when players are in range
    [SerializeField] private bool cyclingSpawns;

    [Header("Spawning Variables")]
    [SerializeField] private float minCycleLength = 5f;
    [SerializeField] private float maxCycleLength = 10f;

    [Header("References")]
    public GameObject tentacle;
    [SerializeField] private Animator animator;
    [SerializeField] private NewTentacleAttack newTentacleAttack;

    public bool shipInRange;
    private bool isUp;
    private bool isAnimating;
    private bool waitingToFall;
    private bool waitingToRise;
    private bool isDefeated;
    private Coroutine fallCoroutine;
    private float currentCycleTime;
    private bool isAttacking;


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
        if (isAttacking) 
        {
            return;
        }
        if (isDefeated) 
        {
            TentacleSlayed();
        }

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
        if (!isAttacking)
        {
            if ((other.CompareTag("Ship") && rangeSpawned && !shipInRange) || (other.CompareTag("Ship") && cyclingSpawns && !shipInRange))
            {
                shipInRange = true;
                TentacleArise();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isAttacking)
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
    }

    private void Initialize()
    {
        isUp = false;
        shipInRange = false;
        isAnimating = false;
        waitingToFall = false;
        waitingToRise = false;
        isDefeated = false;
        isAttacking = false;

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

        if (!isAnimating && !tentacle.activeSelf && !isAttacking)
        {
            TentacleArise();
        }
    }

    IEnumerator TentFall()
    {
        isAnimating = true;
        animator.SetBool("isUp", false);


        if (isAttacking)
        {
            newTentacleAttack.StartAttack();
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isUp = false;
        isAnimating = false;
        waitingToFall = false;
        tentacle.SetActive(false);

        if (isDefeated)
        {
            this.gameObject.SetActive(false);
        }

        currentCycleTime = Random.Range(minCycleLength, maxCycleLength);

        if (shipInRange && !isDefeated && !isAttacking)
        {
            TentacleArise();
        }
    }

    public void TentacleSlayed()
    {
        isDefeated = true;
        if (isUp)
        {
            TentacleSubmerge();
        }
        else
        {
            StartCoroutine(TentFall());
        }
    }

    public void TentacleAttackStart()
    {
        newTentacleAttack.StartGrab();
        isAttacking = true;
        if (isUp)
        {
            TentacleSubmerge();
        }
        else
        {
            StartCoroutine(TentFall());
        }
    }

}
