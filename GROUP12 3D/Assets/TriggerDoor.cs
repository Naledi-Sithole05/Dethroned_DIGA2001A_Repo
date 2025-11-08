using UnityEngine;
using TMPro;

public class DoorInteraction : MonoBehaviour
{
    [Header("References")]
    public Animator doorAnimator;
    public TextMeshProUGUI interactionText;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip doorOpenSound;

    [Header("Settings")]
    public bool isOpen = false;
    public float closeDelay = 1f; // Time after leaving before door closes

    private bool playerInside = false;
    private Coroutine closeCoroutine;

    private void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        // Auto-assign AudioSource if missing
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        if (!isOpen)
        {
            // Open the door
            doorAnimator.SetTrigger(openTrigger);
            isOpen = true;
            Debug.Log("Door opened automatically.");

            // Play door open sound
            if (audioSource != null && doorOpenSound != null)
                audioSource.PlayOneShot(doorOpenSound);

            if (interactionText != null)
            {
                interactionText.text = "Door Opening...";
                interactionText.gameObject.SetActive(true);
            }
        }

        // Stop any close coroutine (if player re-enters quickly)
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        // Delay the close a bit — ensures player fully left area
        if (closeCoroutine != null)
            StopCoroutine(closeCoroutine);
        closeCoroutine = StartCoroutine(CloseDoorAfterDelay());
    }

    private System.Collections.IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        if (!playerInside && isOpen)
        {
            doorAnimator.SetTrigger(closeTrigger);
            isOpen = false;
            Debug.Log("Door closed automatically after player left.");

            if (interactionText != null)
            {
                interactionText.text = "Door Closing...";
                interactionText.gameObject.SetActive(false);
            }
        }
    }
}
