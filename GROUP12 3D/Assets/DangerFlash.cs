using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    [Range(0f, 1f)] public float maxAlpha = 0.6f;
    public float fadeOutTime = 0.5f;

    private Image bloodImage;
    private Coroutine currentFlash;

    void Awake()
    {
        bloodImage = GetComponent<Image>();
        SetAlpha(0f);
    }

    public void Flash()
    {
        if (currentFlash != null)
            StopCoroutine(currentFlash);
        currentFlash = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Instantly show the blood image
        SetAlpha(maxAlpha);

        // Then fade out over time
        float elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(maxAlpha, 0f, elapsed / fadeOutTime);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = bloodImage.color;
        c.a = alpha;
        bloodImage.color = c;
    }
}
