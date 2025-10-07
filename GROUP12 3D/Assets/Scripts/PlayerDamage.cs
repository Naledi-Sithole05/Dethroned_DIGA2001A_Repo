using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("References")]
    public Healthbar healthbar;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
        healthbar.SetHealth(currentHealth);
    }

    void OnTriggerEnter(Collider other)
    {
        // Damage player when touching any of these tagged objects
        if (other.CompareTag("Trap") ||
            other.CompareTag("Beam") ||
            other.CompareTag("Wrecking Ball") ||
            other.CompareTag("Wall"))   // 
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

