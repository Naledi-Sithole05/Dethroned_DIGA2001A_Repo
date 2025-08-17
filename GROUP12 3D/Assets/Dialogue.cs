 using System.Collections; // Required for IEnumerator
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textComponent; // Assign in Inspector
    public GameObject dialoguePanel; // Optional: for hiding/showing panel

    [Header("Dialogue Settings")]
    [TextArea(3, 10)] public string[] lines; // Multi-line text fields
    public float textSpeed = 0.05f;
    public bool allowMouseSkip = true;
    public KeyCode advanceKey = KeyCode.Space;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private int index;
    private bool isTyping;

    void Start()
    {
        ValidateReferences();
        InitializeDialogue();
    }

    void Update()
    {
        if (CanAdvanceDialogue())
        {
            HandleInput();
        }
    }

    // ===== SETUP METHODS =====
    private void ValidateReferences()
    {
        if (textComponent == null)
        {
            Debug.LogError("TextComponent not assigned in Dialogue script!", this);
            enabled = false;
        }

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned!", this);
        }
    }

    private void InitializeDialogue()
    {
        textComponent.text = string.Empty;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        onDialogueStart?.Invoke();
        StartDialogue();
    }

    // ===== INPUT HANDLING =====
    private bool CanAdvanceDialogue()
    {
        return !isTyping || (allowMouseSkip && Input.GetMouseButtonDown(0)) || Input.GetKeyDown(advanceKey);
    }

    private void HandleInput()
    {
        if (textComponent.text == lines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }

    // ===== DIALOGUE FLOW =====
    public void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        textComponent.text = string.Empty;

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        onDialogueEnd?.Invoke();
    }

    // ===== PUBLIC METHODS =====
    public void SkipToEnd()
    {
        StopAllCoroutines();
        textComponent.text = lines[index];
        isTyping = false;
    }

    public void SetLines(string[] newLines)
    {
        lines = newLines;
        index = 0;
        StartDialogue();
    }
}