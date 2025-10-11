using UnityEngine;
using TMPro; // Use this for TextMeshProUGUI

public class DoorInteraction : MonoBehaviour
{
    [Header("References")]
    public Animator doorAnimator;
    public TextMeshProUGUI interactionText; // TMP text reference
    public string openTrigger = "Open";
    public string closeTrigger = "Close";

    [Header("Settings")]
    public bool isOpen = false; // Tracks whether the door is open
    private bool playerIsNear = false;

    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerIsNear)
        {
            if (interactionText != null)
                interactionText.text = isOpen ? "Press 'O' to Close" : "Press 'O' to Open";

            if (Input.GetKeyDown(KeyCode.O))
            {
                if (isOpen)
                {
                    doorAnimator.SetTrigger(closeTrigger);
                    isOpen = false;
                }
                else
                {
                    doorAnimator.SetTrigger(openTrigger);
                    isOpen = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            if (interactionText != null)
                interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }
}
