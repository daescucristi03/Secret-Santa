using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    [Header("General AI Settings")]
    public float roamSpeed = 3.5f;
    public float chaseSpeed = 6f;
    public float sightRange = 15f;
    public float fieldOfView = 90f;
    public float giftChangeInterval = 5f;

    [Header("Hidden Gift Locations")]
    public Transform[] hiddenGiftLocations;

    [Header("Player Settings")]
    public GameObject player;
    public Transform respawnPoint;

    [Header("UI Settings")]
    public Image fadeScreen;
    public float fadeDuration = 1.5f;

    private NavMeshAgent agent;
    private bool isCatchingPlayer = false;
    private bool isChasing = false;
    private float giftChangeTimer;

    private Dictionary<GameObject, Transform> giftToSpotMap = new Dictionary<GameObject, Transform>(); // Tracks assigned spots
    private HashSet<Transform> usedSpots = new HashSet<Transform>(); // Tracks used hiding spots

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent component found on the AI!");
        }

        giftChangeTimer = giftChangeInterval;

        if (fadeScreen != null)
        {
            Color color = fadeScreen.color;
            color.a = 0f;
            fadeScreen.color = color;
            fadeScreen.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isCatchingPlayer) return;

        if (CanSeePlayer())
        {
            ChasePlayer();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                agent.speed = roamSpeed;
            }

            RoamAndManageGifts();
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > sightRange) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfView / 2) return false;

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, sightRange))
        {
            if (hit.transform.gameObject == player)
            {
                return true;
            }
        }

        return false;
    }

    void ChasePlayer()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) < 1.5f)
        {
            StartCoroutine(CatchPlayer());
        }
    }

    void RoamAndManageGifts()
    {
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            SetRandomRoamDestination();
        }
    }


    public void OnGiftDestroyed(GameObject gift)
    {
        if (giftToSpotMap.ContainsKey(gift))
        {
            // Free up the hiding spot
            Transform freedSpot = giftToSpotMap[gift];
            usedSpots.Remove(freedSpot);

            // Remove the gift from the map
            giftToSpotMap.Remove(gift);

            Debug.Log($"Hiding spot {freedSpot.name} is now available.");
        }
    }

    void SetRandomRoamDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * sightRange;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, sightRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    IEnumerator CatchPlayer()
    {
        if (isCatchingPlayer) yield break;

        isCatchingPlayer = true;

        agent.isStopped = true;

        yield return StartCoroutine(FadeScreen(true));

        TeleportPlayer();

        yield return StartCoroutine(FadeScreen(false));

        agent.isStopped = false;
        isCatchingPlayer = false;
    }


    void TeleportPlayer()
    {

        SceneManager.LoadSceneAsync("Level_Main");
        // if (player == null || respawnPoint == null)
        // {
        //     Debug.LogError("Player or Respawn Point is not assigned!");
        //     return;
        // }

        // Transform playerTransform = player.transform;

        // playerTransform.position = respawnPoint.position;

        // Rigidbody playerRb = player.GetComponent<Rigidbody>();
        // if (playerRb != null)
        // {
        //     playerRb.linearVelocity = Vector3.zero;
        //     ResetPlayerAndGifts();
        // }

        // CharacterController playerController = player.GetComponent<CharacterController>();
        // if (playerController != null)
        // {
        //     playerController.enabled = false;
        //     playerTransform.position = respawnPoint.position;
        //     playerController.enabled = true;
        // }
    }

    IEnumerator FadeScreen(bool fadeToBlack)
    {
        if (fadeScreen == null) yield break;

        fadeScreen.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color color = fadeScreen.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = fadeToBlack
                ? Mathf.Clamp01(elapsedTime / fadeDuration)
                : Mathf.Clamp01(1f - elapsedTime / fadeDuration);

            color.a = alpha;
            fadeScreen.color = color;

            yield return null;
        }

        if (!fadeToBlack)
        {
            fadeScreen.gameObject.SetActive(false);
        }
    }
}
