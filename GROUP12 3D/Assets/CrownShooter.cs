using UnityEngine;
using System.Collections;

public class CrownShooter : MonoBehaviour
{
    [Header("References")]
    public GameObject player;                // Player target
    public GameObject projectilePrefab;      // Projectile prefab (must have Rigidbody)
    public Transform shootPoint;             // Where projectiles spawn
    public AudioSource shootAudio;           // Audio source for cannon
    public AudioClip shootClip;              // Fire sound effect

    [Header("Shooting Settings")]
    public float shootForce = 20f;           // Force applied to projectiles
    public float timeBetweenShots = 1f;      // Delay between shots
    public float shootDuration = 10f;        // How long the cannon shoots for

    [Header("Projectile Settings")]
    public float projectileLifetime = 3f;    // How long projectiles exist before being destroyed

    private bool isActivated = false;
    private bool isShooting = false;

    private void Update()
    {
        // Start the shooting coroutine once activated
        if (isActivated && !isShooting)
        {
            StartCoroutine(ShootContinuously());
        }
    }

    /// <summary>
    /// Called externally when the player picks up the crown.
    /// </summary>
    public void ActivateShooter()
    {
        if (isActivated) return; // Prevent double-activation

        isActivated = true;
        Debug.Log("CrownShooter activated! Cannon firing started.");
    }

    private IEnumerator ShootContinuously()
    {
        isShooting = true;
        float timer = 0f;

        while (timer < shootDuration)
        {
            ShootAtPlayer();
            yield return new WaitForSeconds(timeBetweenShots);
            timer += timeBetweenShots;
        }

        isShooting = false;
        isActivated = false;
    }

    private void ShootAtPlayer()
    {
        if (player == null || projectilePrefab == null || shootPoint == null)
            return;

        Vector3 direction = (player.transform.position - shootPoint.position).normalized;

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Updated for Unity 2025+
            rb.linearVelocity = direction * shootForce;
        }

        Destroy(projectile, projectileLifetime);

        // Play firing sound
        if (shootAudio != null && shootClip != null)
            shootAudio.PlayOneShot(shootClip);

        Debug.Log("CrownShooter fired at player!");
    }

}
