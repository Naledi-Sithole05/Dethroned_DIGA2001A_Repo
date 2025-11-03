using UnityEngine;

public class BoulderShooter : MonoBehaviour
{
    [Header("Boulder Settings")]
    public GameObject boulderPrefab;
    public Transform spawnPoint;
    public float boulderSpeed = 10f;
    public float fireRate = 2f;
    public float boulderLifetime = 5f;

    [Header("Checkpoint Trigger")]
    public Collider checkpoint4Trigger;
    public string playerTag = "Player";

    private bool isActive = false;
    private float fireTimer = 0f;

    void Start()
    {
        if (checkpoint4Trigger != null)
            checkpoint4Trigger.gameObject.AddComponent<CheckpointTrigger>().Setup(this, playerTag);
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

    void FireBoulder()
    {
        if (boulderPrefab == null || spawnPoint == null) return;

        GameObject boulder = Instantiate(boulderPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = boulder.GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = spawnPoint.forward * boulderSpeed; // Unity 6+ compatible

        Destroy(boulder, boulderLifetime);
    }

    public void ActivateShooter()
    {
        isActive = true;
    }
}

public class CheckpointTrigger : MonoBehaviour
{
    private BoulderShooter shooter;
    private string playerTag;

    public void Setup(BoulderShooter shooterRef, string tag)
    {
        shooter = shooterRef;
        playerTag = tag;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            shooter.ActivateShooter();
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        
    }
}//jfjbkfbk;
// inknfnma gvarbgmrjgqergr
//kihefubwygfi  wbgh9   rg  wr
//burbfjwrf
//huwebf
//hebfjbe
//jhrkvbkwr
//jhuwebf
