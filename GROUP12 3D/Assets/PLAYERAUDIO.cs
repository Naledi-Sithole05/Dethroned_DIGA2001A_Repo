using UnityEngine;

public class PlayerWalkAudio : MonoBehaviour
{
    public CharacterController characterController; // Drag your CharacterController here
    public AudioSource walkAudio;                   // Drag your walking AudioSource here
    public float moveThreshold = 0.1f;              // How sensitive the movement detection is

    void Update()
    {
        if (characterController == null || walkAudio == null)
            return;

        // Check if the player is moving
        if (characterController.velocity.magnitude > moveThreshold && characterController.isGrounded)
        {
            // Start playing if not already playing
            if (!walkAudio.isPlaying)
                walkAudio.Play();
        }
        else
        {
            // Stop sound when not moving
            if (walkAudio.isPlaying)
                walkAudio.Stop();
        }
    }
}
