using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject playerObject;       // Drag the player here
    public int maxHealth = 5;
    public Healthbar healthbar;           // Drag the Healthbar script here

    [Header("Hazards")]
    public Collider[] hazards;            // Drag hazard colliders here (trap cubes, walls, lasers)

    private Rigidbody playerRigidbody;
    private Vector3 startPosition;
    private int currentHealth;

    void Start()
    {
        if (playerObject == null)
        {
            Debug.LogError("Player GameObject not assigned!");
            return;
        }

        playerRigidbody = playerObject.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogError("No Rigidbody found on the Player!");
            return;
        }

        startPosition = playerObject.transform.position;
        currentHealth = maxHealth;

        if (healthbar != null)
        {
            healthbar.SetMaxHealth(maxHealth);
            healthbar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogError("Healthbar not assigned in inspector!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (Collider hazard in hazards)
        {
            if (hazard != null && collision.collider == hazard)
            {
                TakeDamage(1);
                break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (Collider hazard in hazards)
        {
            if (hazard != null && other == hazard)
            {
                TakeDamage(1);
                break;
            }
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthbar != null)
        {
            healthbar.SetHealth(currentHealth);
        }
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            ResetPlayer();
        }
    }

    void ResetPlayer()
    {
        // Move player back to start
        playerObject.transform.position = startPosition;

        // Stop all motion
#if UNITY_6000_0_OR_NEWER
        playerRigidbody.linearVelocity = Vector3.zero;
#else
        playerRigidbody.velocity = Vector3.zero;
#endif
        playerRigidbody.angularVelocity = Vector3.zero;

        // Restore health
        currentHealth = maxHealth;
        if (healthbar != null)
        {
            healthbar.SetHealth(currentHealth);
        }
        Debug.Log("Player reset! Health restored.");
    }
}
