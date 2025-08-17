 using UnityEngine;

public class PickUpCrown : MonoBehaviour
{
    public GameObject pickUpText; // The "Press E" UI prompt
    public GameObject CrownOnPlayer; // The crown that appears on the player when picked up

    void Start()
    {
        pickUpText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUpText.SetActive(true);

            // Changed to GetKeyDown so it triggers once per press
            if (Input.GetKeyDown(KeyCode.E))
            {
                gameObject.SetActive(false); // Hide the crown in the world
                CrownOnPlayer.SetActive(true); // Show the crown on the player
                pickUpText.SetActive(false); // Hide the prompt
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
}
