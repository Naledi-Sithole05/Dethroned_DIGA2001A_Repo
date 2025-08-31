using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject pickUpEffect;
    public int healAmount = 2; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                
                if (playerHealth != null)
                {
                    playerHealth.Heal(healAmount);
                }
            }

            Pickup();
        }
    }

    void Pickup()
    {
        if (pickUpEffect != null)
        {
            Instantiate(pickUpEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject); 
    }
}
