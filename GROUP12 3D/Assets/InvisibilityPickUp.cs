using System.Collections;
using UnityEngine;
using TMPro;

public class InvisibilityPickup : MonoBehaviour
{
    [Header("Invisibility Settings")]
    [Tooltip("How long the player stays undetectable after picking this up.")]
    public float invisibilityDuration = 5f;

    [Header("Respawn Settings")]
    [Tooltip("Time before this invisibility pickup respawns (in seconds).")]
    public float respawnTime = 300f; // Default 5 minutes

    [Header("Visual & Audio Effects")]
    [Tooltip("Effect that plays when the player picks this up.")]
    public GameObject pickUpEffect;

    [Tooltip("Effect that plays when the pickup respawns.")]
    public GameObject respawnEffect;

    [Header("UI Settings (Optional)")]
    [Tooltip("Text prefab to show when collected (e.g., 'Invisibility!')")]
    public GameObject floatingTextPrefab;

    [Tooltip("Where to spawn the text above the pickup.")]
    public Vector3 textOffset = new Vector3(0, 2f, 0);

    private Collider col;
    private Renderer rend;

    private void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Activate invisibility on player
            FPController player = other.GetComponent<FPController>();
            if (player != null)
            {
                player.ActivateInvisibility(invisibilityDuration);
            }

            ShowFloatingText();
            StartCoroutine(HandlePickup());
        }
    }

    private void ShowFloatingText()
    {
        if (floatingTextPrefab != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, transform.position + textOffset, Quaternion.identity);
            TextMeshPro tmp = textObj.GetComponentInChildren<TextMeshPro>();

            if (tmp != null)
                tmp.text = "Invisibility Activated!";
        }
    }

    private IEnumerator HandlePickup()
    {
       
        if (pickUpEffect != null)
            Instantiate(pickUpEffect, transform.position, Quaternion.identity);

     
        rend.enabled = false;
        col.enabled = false;

        
        yield return new WaitForSeconds(respawnTime);

        
        rend.enabled = true;
        col.enabled = true;

        
        if (respawnEffect != null)
            Instantiate(respawnEffect, transform.position, Quaternion.identity);
    }
}
