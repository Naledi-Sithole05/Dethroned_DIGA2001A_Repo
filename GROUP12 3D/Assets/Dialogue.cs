  using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textComponent;
    public GameObject dialoguePanel;

    [Header("Dialogue Settings")]
    [TextArea(3, 10)] public string[] lines;
    public float textSpeed = 0.05f;
    public bool allowMouseSkip = true;
    public KeyCode advanceKey = KeyCode.Space;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private int index;
    private bool isTyping;
    private Coroutine typingCoroutine;

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

    #region Setup
    private void ValidateReferences()
    {
        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent == null)
                Debug.LogError("No TextMeshProUGUI component found!", this);
        }

        if (dialoguePanel == null)
            dialoguePanel = gameObject;
    }

    private void InitializeDialogue()
    {
        textComponent.text = string.Empty;
        SetDialogueActive(true);
        SafeInvoke(onDialogueStart);
        StartDialogue();
    }
    #endregion

    #region Input
    private bool CanAdvanceDialogue()
    {
        return Input.GetKeyDown(advanceKey) || 
               (allowMouseSkip && Input.GetMouseButtonDown(0));
    }

    private void HandleInput()
    {
        if (isTyping)
            SkipToEnd();
        else
            NextLine();
    }
    #endregion

    #region Dialogue Flow
    public void StartDialogue()
    {
        index = 0;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
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

    private void SkipToEnd()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textComponent.text = lines[index];
        isTyping = false;
    }

    public void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        SetDialogueActive(false);
        SafeInvoke(onDialogueEnd);
    }
    #endregion

    #region Utilities
    private void SetDialogueActive(bool state)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(state);
    }

    // Completely safe event invocation
    private void SafeInvoke(UnityEvent unityEvent)
    {
        if (unityEvent == null) return;

        try
        {
            // Only invoke if there are persistent listeners
            if (unityEvent.GetPersistentEventCount() > 0)
            {
                unityEvent.Invoke();
            }
            else
            {
                // For runtime-added listeners
                var method = unityEvent.GetType().GetMethod("GetDelegateCount");
                if (method != null && (int)method.Invoke(unityEvent, null) > 0)
                {
                    unityEvent.Invoke();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Event invocation failed: {e.Message}");
        }
    }
    #endregion

    #region Public API
    public void SetLines(string[] newLines)
    {
        lines = newLines;
        index = 0;
        StartDialogue();
    }

    public void ForceEndDialogue()
    {
        SkipToEnd();
        EndDialogue();
    }
    #endregion
}