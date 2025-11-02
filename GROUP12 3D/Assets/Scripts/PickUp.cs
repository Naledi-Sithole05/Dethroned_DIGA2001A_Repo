using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUpCrown : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pickUpText;      // Text displayed when near crown
    public GameObject exitText;        // Text telling player to exit castle
    public GameObject CrownOnPlayer;   // Crown object that will appear on player

    [Header("Audio Settings")]
    public AudioSource crownAudio;

    [Header("Scene Settings")]
    public string nextSceneName = "Level2";

    [Header("Shooter Settings")]
    public CrownShooter shooter;

    private bool hasCrown = false;

    private void Start()
    {
        if (pickUpText != null)
            pickUpText.SetActive(false);

        if (exitText != null)
            exitText.SetActive(false);

        if (CrownOnPlayer != null)
            CrownOnPlayer.SetActive(false);

        if (crownAudio != null)
            crownAudio.playOnAwake = false;
    }

    /// <summary>
    /// Call this from FPController when player presses the pickup button while in range
    /// </summary>
    public void PickUp()
    {
        if (hasCrown)
            return;

        if (crownAudio != null && crownAudio.clip != null)
            AudioSource.PlayClipAtPoint(crownAudio.clip, transform.position);

        if (CrownOnPlayer != null)
            CrownOnPlayer.SetActive(true);

        if (pickUpText != null)
            pickUpText.SetActive(false);

        if (exitText != null)
            exitText.SetActive(true);

        hasCrown = true;

        if (shooter != null)
            shooter.ActivateShooter();

        gameObject.SetActive(false); // hide the world crown
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasCrown)
        {
            if (pickUpText != null)
                pickUpText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasCrown && pickUpText != null)
            pickUpText.SetActive(false);
    }

    public void CompleteMission()
    {
        if (hasCrown)
            SceneManager.LoadScene(nextSceneName);
    }
}
