using UnityEngine;
using System.Collections;

public class CrownShooter : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public AudioSource shootAudio; // Add your audio source here

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
        RaycastHit hit;

        if (Physics.Raycast(shootPoint.position, direction, out hit))
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = direction * shootForce;
            }

            // Play the shooting sound
            if (shootAudio != null)
            {
                shootAudio.Play();
            }

            Destroy(projectile, projectileLifetime);
        }
    }
}
