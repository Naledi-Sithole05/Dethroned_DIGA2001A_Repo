using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUpCrown : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pickUpText;       // Text displayed when near crown
    public GameObject exitText;         // Text telling player to exit castle
    public GameObject CrownOnPlayer;    // Optional reference if you have a visible crown on player

    [Header("Audio Settings")]
    public AudioSource crownAudio;      // Assign clip here

    [Header("Scene Settings")]
    public string nextSceneName = "Level2";

    [Header("Shooter Reference")]
    public CrownShooter shooter;        // Reference to your cannon shooter

    private bool hasCrown = false;
    private bool playerInRange = false;
    private Transform playerTransform;

    // Variables for "world-locked follow" behavior
    private Vector3 pickupOffset;
    private Quaternion worldRotation;

    private void Start()
    {
        if (pickUpText != null)
            pickUpText.SetActive(false);

        if (exitText != null)
            exitText.SetActive(false);

        if (crownAudio != null)
            crownAudio.playOnAwake = false;
    }

    private void Update()
    {
        // Follow player's position once the crown is picked up
        if (hasCrown && playerTransform != null)
        {
            transform.position = playerTransform.position + pickupOffset;
            transform.rotation = worldRotation;
        }

        // Check pickup input when player is in range
        if (playerInRange && !hasCrown && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasCrown)
        {
            playerInRange = true;
            playerTransform = other.transform;

            if (pickUpText != null)
                pickUpText.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasCrown && pickUpText != null)
            pickUpText.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasCrown)
        {
            playerInRange = false;
            if (pickUpText != null)
                pickUpText.SetActive(false);
        }
    }

    private void PickUp()
    {
        // Play pickup sound
        if (crownAudio != null && crownAudio.clip != null)
            AudioSource.PlayClipAtPoint(crownAudio.clip, transform.position);

        // UI changes
        if (pickUpText != null)
            pickUpText.SetActive(false);
        if (exitText != null)
            exitText.SetActive(true);

        // Optional: if you have a player crown object
        if (CrownOnPlayer != null)
            CrownOnPlayer.SetActive(false);

        // Disable collider so it can't be picked up again
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Save current offset from player and rotation so it follows properly
        pickupOffset = transform.position - playerTransform.position;
        worldRotation = transform.rotation;

        hasCrown = true;

        //  Activate the cannon shooter
        if (shooter != null)
            shooter.ActivateShooter();

        Debug.Log("Crown picked up: now moves with player but ignores camera rotation. Shooter activated.");
    }

    // Trigger for mission completion
    public void CompleteMission()
    {
        if (hasCrown)
            SceneManager.LoadScene(nextSceneName);
    }
}
