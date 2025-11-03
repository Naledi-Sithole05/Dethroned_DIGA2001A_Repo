using UnityEngine;

public class Boulder : MonoBehaviour
{
    [Header("Boulder Settings")]
    public GameObject boulderPrefab;      // Boulder prefab with Rigidbody
    public Transform spawnPoint;          // Where boulders spawn from
    public float boulderSpeed = 10f;      // Launch speed
    public float fireRate = 1.5f;         // Time between boulders
    public float boulderLifetime = 5f;    // How long before a boulder disappears

    [Header("Checkpoint Settings")]
    public Collider checkpoint4Trigger;   // Trigger collider for checkpoint 4
    public string playerTag = "Player";   // Player tag for detection

    private bool isActive = false;        // When true, the cannon starts firing
    private float fireTimer = 0f;

    void Start()
    {
        if (checkpoint4Trigger != null)
        {
            // Ensure trigger collider is set correctly
            if (!checkpoint4Trigger.isTrigger)
                checkpoint4Trigger.isTrigger = true;

            // Add helper trigger script
            CheckpointActivator triggerScript = checkpoint4Trigger.gameObject.AddComponent<CheckpointActivator>();
            triggerScript.Setup(this, playerTag);
        }
    }

    void Update()
    {
        if (!isActive) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            FireBoulder();
            fireTimer = 0f;
        }
    }

    private void FireBoulder()
    {
        if (boulderPrefab == null || spawnPoint == null) return;

        // Spawn the boulder
        GameObject boulder = Instantiate(boulderPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = boulder.GetComponent<Rigidbody>();

        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = spawnPoint.forward * boulderSpeed;
#else
            rb.velocity = spawnPoint.forward * boulderSpeed;
#endif
        }

        // Destroy the boulder after its lifetime
        Destroy(boulder, boulderLifetime);
    }

    public void ActivateShooter()
    {
        isActive = true;
    }
}

public class CheckpointActivator : MonoBehaviour
{
    private Boulder boulderShooter;
    private string playerTag;

    public void Setup(Boulder shooterRef, string tag)
    {
        boulderShooter = shooterRef;
        playerTag = tag;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            boulderShooter.ActivateShooter();
            gameObject.SetActive(false); // Disable trigger so it only activates once
        }
    }
}
