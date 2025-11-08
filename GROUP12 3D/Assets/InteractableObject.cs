using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true; // Make sure this collider is for detection only
    }

    private void OnTriggerEnter(Collider other)
    {
        FPController player = other.GetComponent<FPController>();
        if (player != null)
        {
            player.SetNearbyObject(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FPController player = other.GetComponent<FPController>();
        if (player != null)
        {
            player.ClearNearbyObject(this);
        }
    }

    public void HidePrompt()
    {
        
        FPController player = Object.FindFirstObjectByType<FPController>();
        if (player != null)
        {
            player.ClearNearbyObject(this);
        }
    }
}
