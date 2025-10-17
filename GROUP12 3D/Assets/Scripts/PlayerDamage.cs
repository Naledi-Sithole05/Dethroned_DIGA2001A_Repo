using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("References")]
    public Healthbar healthbar;
    private Transform wreckingBall;
    private float wreckingBallDamageRange = 2.5f; // How close the ball needs to be to deal damage
    private float wreckingBallDamageCooldown = 1f; // Seconds between hits
    private float nextDamageTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
        healthbar.SetHealth(currentHealth);

        // Find the wrecking ball in the scene by tag
        GameObject wb = GameObject.FindGameObjectWithTag("Wrecking Ball");
        if (wb != null)
            wreckingBall = wb.transform;
        else
            Debug.LogWarning("No object with tag 'Wrecking Ball' found in scene!");
    }

    void Update()
    {
        DetectWreckingBall();

        // You can keep other Update logic here
    }

    void DetectWreckingBall()
    {
        if (wreckingBall == null) return;

        // Perform a raycast between wrecking ball and player
        Vector3 direction = (wreckingBall.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, wreckingBall.position);

        if (distance <= wreckingBallDamageRange)
        {
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            // If nothing blocks the line between them
            if (Physics.Raycast(ray, out hit, wreckingBallDamageRange))
            {
                if (hit.collider.CompareTag("Wrecking Ball") && Time.time >= nextDamageTime)
                {
                    TakeDamage(1);
                    nextDamageTime = Time.time + wreckingBallDamageCooldown;
                    Debug.Log("Player hit by wrecking ball via raycast!");
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Handle other traps normally
        if (other.CompareTag("Trap") || other.CompareTag("Beam"))
        {
            TakeDamage(1);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthbar.SetHealth(currentHealth);

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

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
