 using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GuardAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;

    [Header("Detection Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 50f;
    [SerializeField] private string gameOverSceneName = "GameOver";

    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private bool isActive = false;
    private Renderer[] renderers;

    // Animator variables
    private Animator animator;
    private int isWalkingHash;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("GuardAI: Missing NavMeshAgent.");

        // Get animator from parent (since it's on GuardPrefab)
        animator = GetComponentInParent<Animator>();
        if (animator == null)
            Debug.LogError("GuardAI: Could not find Animator in parent.");

        isWalkingHash = Animator.StringToHash("isWalking");

        renderers = GetComponentsInChildren<Renderer>();
        SetGuardVisible(false);
        
        // Don't set isStopped here - wait for Start or activation
    }

    void Start()
    {
        // Initialize agent state in Start instead of Awake
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }
    }

    void Update()
    {
        if (!isActive) return;

        Patrol();
        DetectPlayer();
    }

    public void ActivateGuard()
    {
        if (agent == null) return;

        isActive = true;
        
        // Only set isStopped if agent is properly placed on NavMesh
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        // Make guard visible
        SetGuardVisible(true);

        // Trigger walking animation
        if (animator != null)
            animator.SetBool(isWalkingHash, true);

        // Start patrolling
        if (waypoints.Length > 0 && agent.isOnNavMesh)
        {
            currentWaypoint = 0;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }

        Debug.Log("Guard activated - starting patrol.");
    }

    void Patrol()
    {
        if (waypoints.Length == 0 || agent == null || !agent.isOnNavMesh) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Debug.Log("Player detected! Loading game over scene...");
            if (!string.IsNullOrEmpty(gameOverSceneName))
                SceneManager.LoadScene(gameOverSceneName);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void SetGuardVisible(bool visible)
    {
        if (renderers == null) return;

        foreach (Renderer rend in renderers)
            rend.enabled = visible;
    }
}
