using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUpCrown : MonoBehaviour
{
    public GameObject pickUpText;
    public GameObject CrownOnPlayer;
    public string nextSceneName = "Level2";
    public AudioSource crownAudio; // Assign clip here

    private void Start()
    {
        if (pickUpText != null)
            pickUpText.SetActive(false);

        if (crownAudio != null)
            crownAudio.playOnAwake = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickUpText != null)
                pickUpText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Play audio safely even after deactivating crown
                if (crownAudio != null && crownAudio.clip != null)
                {
                    AudioSource.PlayClipAtPoint(crownAudio.clip, transform.position);
                }

                gameObject.SetActive(false);

                if (CrownOnPlayer != null)
                    CrownOnPlayer.SetActive(true);

                if (pickUpText != null)
                    pickUpText.SetActive(false);

                // Load next scene after clip length
                float delay = crownAudio != null && crownAudio.clip != null
                                ? crownAudio.clip.length
                                : 1f;
                Invoke(nameof(LoadNextScene), delay);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && pickUpText != null)
            pickUpText.SetActive(false);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
