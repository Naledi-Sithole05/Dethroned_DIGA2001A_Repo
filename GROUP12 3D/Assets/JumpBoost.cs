using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpBoostPickup : MonoBehaviour
{
    [Tooltip("Time before this object reappears (in seconds).")]
    public float respawnTime = 300f; // 5 minutes

    [Tooltip("Optional: Visual effect to play on pickup.")]
    public GameObject pickupEffect;

    private Collider col;
    private Renderer rend;

    private void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
    }

    public void StartRespawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        
        rend.enabled = false;
        col.enabled = false;

       
        yield return new WaitForSeconds(respawnTime);

       
        rend.enabled = true;
        col.enabled = true;
    }
}
