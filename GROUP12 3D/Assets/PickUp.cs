 using UnityEngine;
using UnityEngine.SceneManagement; // Add this line

public class PickUpCrown : MonoBehaviour
{
    public GameObject pickUpText;
    public GameObject CrownOnPlayer;
    public string nextSceneName = "Level2"; // Set default or assign in Inspector

    void Start()
    {
        pickUpText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUpText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                gameObject.SetActive(false);
                CrownOnPlayer.SetActive(true);
                pickUpText.SetActive(false);

                // Load next scene after a delay (optional)
                Invoke("LoadNextScene", 1f); // Waits 1 second before loading
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUpText.SetActive(false);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}