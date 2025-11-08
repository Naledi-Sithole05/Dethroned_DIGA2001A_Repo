using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("References")]
    public Healthbar healthbar;
    public DamageFlash bloodFlash;

    private Transform wreckingBall;
    private float wreckingBallDamageRange = 2.5f;
    private float wreckingBallDamageCooldown = 1f;
    private float nextDamageTime = 0f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip beamHitSound;

    private bool isBeamTouching = false; // Tracks continuous beam contact

    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
        healthbar.SetHealth(currentHealth);

        GameObject wb = GameObject.FindGameObjectWithTag("Wrecking Ball");
        if (wb != null)
            wreckingBall = wb.transform;
        else
            Debug.LogWarning("No object with tag 'Wrecking Ball' found in scene!");

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        DetectWreckingBall();

        // If the beam is touching, apply slow continuous damage (optional)
        if (isBeamTouching)
        {
            if (Time.time >= nextDamageTime)
            {
                TakeDamage(1);
                nextDamageTime = Time.time + 1f;
            }
        }
    }

    void DetectWreckingBall()
    {
        if (wreckingBall == null) return;

        Vector3 direction = (wreckingBall.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, wreckingBall.position);

        if (distance <= wreckingBallDamageRange)
        {
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, wreckingBallDamageRange))
            {
                if (hit.collider.CompareTag("Wrecking Ball") && Time.time >= nextDamageTime)
                {
                    TakeDamage(1);
                    nextDamageTime = Time.time + wreckingBallDamageCooldown;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap") || other.CompareTag("Beam") || other.CompareTag("Bullet"))
        {
            TakeDamage(1);

            //  Play beam sound when beam touches player
            if (other.CompareTag("Beam"))
            {
                isBeamTouching = true;
                if (audioSource != null && beamHitSound != null)
                    audioSource.PlayOneShot(beamHitSound);
            }

            if (other.CompareTag("Bullet"))
                Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop beam contact when leaving beam area
        if (other.CompareTag("Beam"))
        {
            isBeamTouching = false;
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar.SetHealth(currentHealth);

        if (bloodFlash != null)
            bloodFlash.Flash();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthbar.SetHealth(currentHealth);
        }
    }

    private void Die()
    {
        CheckpointManager checkpointManager = Object.FindFirstObjectByType<CheckpointManager>();

        if (checkpointManager != null)
        {
            checkpointManager.RespawnPlayer();
            currentHealth = maxHealth;
            healthbar.SetHealth(currentHealth);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
