using UnityEngine;
using System.Collections;

public class CrownShooter : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public AudioSource shootAudio;
    public AudioClip shootClip;

    [Header("Shooting Settings")]
    public float shootForce = 20f;
    public float timeBetweenShots = 1f;
    public float shootDuration = 10f;

    [Header("Projectile Settings")]
    public float projectileLifetime = 3f;

    private bool hasCrown = false;
    private bool isShooting = false;

    void Update()
    {
        if (hasCrown && !isShooting)
        {
            StartCoroutine(ShootContinuously());
        }
    }

    public void ActivateShooter()
    {
        hasCrown = true;
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
    }

    private void ShootAtPlayer()
    {
        if (player == null || projectilePrefab == null || shootPoint == null) return;

        Vector3 direction = (player.transform.position - shootPoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direction * shootForce;
        }

        Destroy(projectile, projectileLifetime);

        // Play the shooting sound manually using PlayOneShot
        if (shootAudio != null && shootClip != null)
        {
            shootAudio.PlayOneShot(shootClip);
        }
    }
}
