using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1.5f;
    public float fadeDuration = 1f;

    private TextMeshPro text;
    private Color originalColor;

    private void Start()
    {
        text = GetComponent<TextMeshPro>();
        originalColor = text.color;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        float t = Mathf.Clamp01((lifetime - Time.timeSinceLevelLoad) / fadeDuration);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
    }
}
