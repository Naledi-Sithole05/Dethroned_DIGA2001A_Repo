using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    private Vector3 respawnPosition; // Last checkpoint position
    private Quaternion respawnRotation;

    private void Start()
    {
        // Set the initial respawn position to the player's starting position
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Update checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            respawnPosition = other.transform.position;
            respawnRotation = other.transform.rotation;
            Debug.Log("Checkpoint updated!");
        }

        // Fall into pit / death trigger
        if (other.CompareTag("Pit"))
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Reset position and rotation
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;

        // Reset Rigidbody velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        // Reset CharacterController position
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            cc.transform.position = respawnPosition;
            cc.enabled = true;
        }

        Debug.Log("Player respawned at last checkpoint!");
    }

}
