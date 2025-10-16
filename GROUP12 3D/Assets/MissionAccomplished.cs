using UnityEngine;

public class MissionAccomplished : MonoBehaviour
{
    public PickUpCrown crownScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            crownScript.CompleteMission();
        }
    }
}
