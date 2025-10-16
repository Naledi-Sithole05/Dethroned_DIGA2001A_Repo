using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GuardAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 20f;

    [Header("Detection Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 50f;
    [SerializeField] private string gameOverSceneName = "GameOver";

    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private bool isActive = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("GuardAI: Missing NavMeshAgent.");
        }
        agent.isStopped = true; // stop movement until activated
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
        agent.isStopped = false;

        if (waypoints.Length > 0)
        {
            currentWaypoint = 0;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }

        Debug.Log("Guard activated — starting patrol.");
    }

    void Patrol()
    {
        if (waypoints.Length == 0 || agent == null) return;

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
}
