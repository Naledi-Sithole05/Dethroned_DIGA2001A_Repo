using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour
{
    [Header("Boulder Settings")]
    public GameObject boulderPrefab;
    public Transform spawnPoint;
    public float boulderSpeed = 15f;
    public float fireRate = 1.5f;
    public float boulderLifetime = 5f;
    public float shootingDuration = 10f;

    [Header("Checkpoint Settings")]
    public Collider checkpoint4Trigger;
    public string playerTag = "Player";

    [Header("Target Settings")]
    public Transform player;

    private bool isActive = false;
    private bool isShooting = false;

    void Start()
    {
        if (checkpoint4Trigger != null)
        {
            if (!checkpoint4Trigger.isTrigger)
                checkpoint4Trigger.isTrigger = true;

            CheckpointActivator triggerScript = checkpoint4Trigger.gameObject.AddComponent<CheckpointActivator>();
            triggerScript.Setup(this, playerTag);
        }
    }

    public void ActivateShooter()
    {
        isActive = true;

        if (!isShooting)
        {
            StartCoroutine(FireBouldersContinuously());
        }
    }

    private IEnumerator FireBouldersContinuously()
    {
        isShooting = true;
        float timer = 0f;

        while (timer < shootingDuration)
        {
            FireBoulder();
            yield return new WaitForSeconds(fireRate);
            timer += fireRate;
        }

        isShooting = false;
    }

    private void FireBoulder()
    {
        if (boulderPrefab == null || spawnPoint == null || player == null) return;

        Vector3 targetPos = player.position + Vector3.up * 1.2f;
        spawnPoint.LookAt(targetPos);

        GameObject boulder = Instantiate(boulderPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = boulder.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearDamping = 0.1f;
            rb.angularDamping = 0.05f;

            rb.linearVelocity = spawnPoint.forward * boulderSpeed;
        }

        Destroy(boulder, boulderLifetime);
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
            gameObject.SetActive(false);
        }
    }
}
