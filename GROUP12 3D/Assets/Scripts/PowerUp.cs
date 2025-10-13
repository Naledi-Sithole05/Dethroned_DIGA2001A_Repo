using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [Tooltip("Amount of health restored to the player.")]
    public int healAmount = 2;

    [Tooltip("Effect that plays when the player picks this up.")]
    public GameObject pickUpEffect;

    [Tooltip("Time before this power-up respawns (in seconds).")]
    public float respawnTime = 300f; // 5 minutes by default

    private Collider col;
    private Renderer rend;

    private void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }

            StartCoroutine(HandlePickup());
        }
    }

    private IEnumerator HandlePickup()
    {
        // Play effect if assigned
        if (pickUpEffect != null)
            Instantiate(pickUpEffect, transform.position, transform.rotation);

        // Hide visuals & disable collision
        rend.enabled = false;
        col.enabled = false;

        // Wait for respawn time
        yield return new WaitForSeconds(respawnTime);

        // Reactivate the object
        rend.enabled = true;
        col.enabled = true;
    }
}
