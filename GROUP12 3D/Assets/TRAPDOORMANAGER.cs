using UnityEngine;
using System.Collections;

public class TrapDoorManager : MonoBehaviour
{
    [Header("Trap Door Settings")]
    public Animator doorAnimator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    public float openDuration = 1.5f;
    public float closeDuration = 1.5f;

    [Header("Respawn Settings")]
    public Transform respawnPoint;       // Where the player returns after falling
    public Collider pitCollider;         // Assign the pit collider here in the Inspector
    public string playerTag = "Player";  // Make sure your player is tagged correctly

    private bool isActivated = false;
    private GameObject player;
    private Vector3 currentRespawnPosition;

    private void Start()
    {
        // Ensure references are assigned
        if (doorAnimator == null)
            Debug.LogWarning("[TrapDoorManager] Missing doorAnimator reference.");

        if (pitCollider == null)
            Debug.LogWarning("[TrapDoorManager] Pit collider not assigned in the Inspector.");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player enters the trap trigger — start door animation loop
        if (other.CompareTag(playerTag))
        {
            player = other.gameObject;
            currentRespawnPosition = respawnPoint != null ? respawnPoint.position : transform.position;

            if (!isActivated)
            {
                isActivated = true;
                StartCoroutine(LoopTrapDoor());
                Debug.Log($"[TrapDoorManager] Trap door animation started on '{gameObject.name}'.");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Continuously check if the player is colliding with the pit (if assigned)
        if (pitCollider != null && other.gameObject == player && other == pitCollider)
        {
            RespawnPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Fallback for non-trigger pit colliders
        if (pitCollider != null && collision.collider == pitCollider)
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("[TrapDoorManager] Player reference missing — cannot respawn.");
            return;
        }

        Debug.Log($"[TrapDoorManager] Player '{player.name}' fell into the pit — respawning...");
        player.transform.position = currentRespawnPosition;
    }

    private IEnumerator LoopTrapDoor()
    {
        while (true)
        {
            doorAnimator.SetTrigger(openTrigger);
            Debug.Log("[TrapDoorManager] Door opening...");
            yield return new WaitForSeconds(openDuration);

            doorAnimator.SetTrigger(closeTrigger);
            Debug.Log("[TrapDoorManager] Door closing...");
            yield return new WaitForSeconds(closeDuration);
        }
    }
}
