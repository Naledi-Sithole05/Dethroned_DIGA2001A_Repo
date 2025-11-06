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

    [Header("Checkpoint Settings")]
    public Collider checkpoint4StartTrigger;  
    public Collider checkpoint4EndTrigger;    
    public string playerTag = "Player";

    [Header("Target Settings")]
    public Transform player;

    [Header("Audio Settings")]
    public AudioSource cannonAudio;   
    public AudioClip fireSound;       

    private bool isShooting = false;
    private Coroutine shootingCoroutine;

    void Start()
    {
        // Setup start trigger
        if (checkpoint4StartTrigger != null)
        {
            if (!checkpoint4StartTrigger.isTrigger)
                checkpoint4StartTrigger.isTrigger = true;

            CheckpointActivator startTriggerScript = checkpoint4StartTrigger.gameObject.AddComponent<CheckpointActivator>();
            startTriggerScript.Setup(this, playerTag, true);  
        }

        // Setup end trigger
        if (checkpoint4EndTrigger != null)
        {
            if (!checkpoint4EndTrigger.isTrigger)
                checkpoint4EndTrigger.isTrigger = true;

            CheckpointActivator endTriggerScript = checkpoint4EndTrigger.gameObject.AddComponent<CheckpointActivator>();
            endTriggerScript.Setup(this, playerTag, false);  
        }
    }

    public void ActivateShooter()
    {
        if (!isShooting)
        {
            isShooting = true;
            shootingCoroutine = StartCoroutine(FireBouldersContinuously());
        }
    }

    public void DeactivateShooter()
    {
        if (isShooting)
        {
            isShooting = false;
            if (shootingCoroutine != null)
                StopCoroutine(shootingCoroutine);
        }
    }

    private IEnumerator FireBouldersContinuously()
    {
        while (isShooting)
        {
            FireBoulder();
            yield return new WaitForSeconds(fireRate);
        }
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

        //  Play cannon firing sound
        if (cannonAudio != null && fireSound != null)
        {
            cannonAudio.PlayOneShot(fireSound);
        }

        Destroy(boulder, boulderLifetime);
    }
}

public class CheckpointActivator : MonoBehaviour
{
    private Boulder boulderShooter;
    private string playerTag;
    private bool startTrigger;

    public void Setup(Boulder shooterRef, string tag, bool isStart)
    {
        boulderShooter = shooterRef;
        playerTag = tag;
        startTrigger = isStart;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (startTrigger)
                boulderShooter.ActivateShooter();
            else
                boulderShooter.DeactivateShooter();
        }
    }
}
