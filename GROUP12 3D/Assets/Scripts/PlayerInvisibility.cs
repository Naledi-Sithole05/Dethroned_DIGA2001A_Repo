using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerInvisibility : MonoBehaviour
{
    [Header("Invisibility Settings")]
    [Tooltip("How long the player stays invisible after pickup.")]
    public float invisibilityDuration = 5f;

    [Tooltip("Transparency")]
    [Range(0f, 1f)]
    public float invisibleAlpha = 0.2f;

    [Header("UI")]
    [Tooltip("Reference")]
    public TextMeshProUGUI invisibilityText;

    private Renderer[] renderers;
    private bool isInvisible = false;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        // Make sure the text starts hidden
        if (invisibilityText != null)
            invisibilityText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InvisibilityPowerUp") && !isInvisible)
        {
            StartCoroutine(BecomeInvisible());
            Destroy(other.gameObject); // remove power-up after pickup
        }
    }

    private IEnumerator BecomeInvisible()
    {
        isInvisible = true;

        SetAlpha(invisibleAlpha);

        // Show Text
        if (invisibilityText != null)
        {
            invisibilityText.text = "Invisibility On!,You're Hidden from the Guard";
            invisibilityText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(invisibilityDuration);

        SetAlpha(1f);
        isInvisible = false;

        // Hide Text
        if (invisibilityText != null)
        {
            invisibilityText.gameObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_Color"))
            {
                Color c = rend.material.color;
                c.a = alpha;
                rend.material.color = c;

                SetMaterialTransparent(rend.material, alpha < 1f);
            }
        }
    }

    private void SetMaterialTransparent(Material mat, bool transparent)
    {
        if (transparent)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        else
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = -1;
        }
    }
}
