using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUpCrown : MonoBehaviour
{
    public GameObject pickUpText;       // Text displayed when near crown
    public GameObject exitText;         // Text telling player to exit castle
    public GameObject CrownOnPlayer;
    public string nextSceneName = "Level2";
    public AudioSource crownAudio;      // Assign clip here
    private bool hasCrown = false;      // Track if player picked up crown

    private void Start()
    {
        if (pickUpText != null)
            pickUpText.SetActive(false);

        if (exitText != null)
            exitText.SetActive(false);

        if (crownAudio != null)
            crownAudio.playOnAwake = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasCrown)
        {
            if (pickUpText != null)
                pickUpText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Play audio safely
                if (crownAudio != null && crownAudio.clip != null)
                {
                    AudioSource.PlayClipAtPoint(crownAudio.clip, transform.position);
                }

                gameObject.SetActive(false);

                if (CrownOnPlayer != null)
                    CrownOnPlayer.SetActive(true);

                if (pickUpText != null)
                    pickUpText.SetActive(false);

                // Show exit text
                if (exitText != null)
                    exitText.SetActive(true);

                hasCrown = true; // Player now has the crown
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasCrown && pickUpText != null)
            pickUpText.SetActive(false);
    }

    // Call this method from your "Mission Accomplished" trigger
    public void CompleteMission()
    {
        if (hasCrown)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
