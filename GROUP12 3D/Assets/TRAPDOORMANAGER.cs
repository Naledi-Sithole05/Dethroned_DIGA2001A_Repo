using UnityEngine;
using System.Collections;

public class TrapDoorManager : MonoBehaviour
{
    [Header("References")]
    public Animator doorAnimator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    public Transform respawnPoint; // Where player returns after falling

    [Header("Settings")]
    public float openDuration = 1.5f;
    public float closeDuration = 1.5f;
    public string pitTag = "Pit"; // Tag for pit collider
    public string playerTag = "Player";

    private bool isActivated = false;
    private GameObject player;
    private Vector3 currentRespawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        // When the player enters the trap trigger
        if (other.CompareTag(playerTag))
        {
            player = other.gameObject;

            // Store respawn position (either custom or this trigger’s position)
            currentRespawnPosition = respawnPoint != null ? respawnPoint.position : transform.position;

            // Start the continuous door animation loop once
            if (!isActivated)
            {
                isActivated = true;
                StartCoroutine(LoopTrapDoor());
                Debug.Log("Trap door animation started!");
            }
        }

        // When the player collides with the pit trigger, teleport back
        else if (other.CompareTag(pitTag) && player != null)
        {
            player.transform.position = currentRespawnPosition;
            Debug.Log("Player fell into pit — respawned at trap trigger.");
        }
    }

    private IEnumerator LoopTrapDoor()
    {
        while (true)
        {
            doorAnimator.SetTrigger(openTrigger);
            yield return new WaitForSeconds(openDuration);

            doorAnimator.SetTrigger(closeTrigger);
            yield return new WaitForSeconds(closeDuration);
        }
    }
}
