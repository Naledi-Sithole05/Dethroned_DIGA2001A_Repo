 using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Assign the TextMeshProUGUI component for dialogue text")]
    public TextMeshProUGUI textComponent;
    
    [Tooltip("Optional panel to toggle visibility")]
    public GameObject dialoguePanel;

    [Header("Dialogue Settings")]
    [TextArea(3, 10)]
    public string[] lines;
    
    [Range(0.001f, 0.1f)]
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

    #region Setup Methods
    private void ValidateReferences()
    {
        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogError("No TextMeshProUGUI component found!", this);
                enabled = false;
                return;
            }
        }

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Dialogue lines array is empty!", this);
        }

        if (dialoguePanel == null)
        {
            dialoguePanel = gameObject;
        }
    }

    private void InitializeDialogue()
    {
        textComponent.text = string.Empty;
        ToggleDialoguePanel(true);
        SafeInvoke(onDialogueStart);
        StartDialogue();
    }
    #endregion

    #region Input Handling
    private bool CanAdvanceDialogue()
    {
        return Input.GetKeyDown(advanceKey) || 
               (allowMouseSkip && Input.GetMouseButtonDown(0));
    }

    private void HandleInput()
    {
        if (isTyping)
        {
            SkipTyping();
        }
        else
        {
            NextLine();
        }
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

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
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
        ToggleDialoguePanel(false);
        SafeInvoke(onDialogueEnd);
    }
    #endregion

    #region Utility Methods
    private void ToggleDialoguePanel(bool state)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(state);
        }
    }

    private void SafeInvoke(UnityEvent unityEvent)
    {
        try
        {
            if (unityEvent != null && unityEvent.GetPersistentEventCount() > 0)
            {
                unityEvent.Invoke();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Event invocation failed: {e.Message}", this);
        }
    }
    #endregion

    #region Public Methods
    public void SetLines(string[] newLines)
    {
        lines = newLines;
        index = 0;
        StartDialogue();
    }

    public void ForceEndDialogue()
    {
        SkipTyping();
        EndDialogue();
    }
    #endregion
}