using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadDoorController : MonoBehaviour
{
    [Header("Trigger Zone")]
    [Tooltip("Assign the trigger collider that the player enters to interact with the keypad.")]
    public Collider keypadTrigger;       // Manually assign this in the Inspector
    public GameObject playerObject;      // Drag your player GameObject here

    [Header("UI Elements")]
    public GameObject keypadPanel;       // The keypad UI panel
    public TMP_InputField pinInputField; // TMP InputField for entering the PIN
    public TMP_Text feedbackText;        // TMP Text for showing feedback
    public Button submitButton;          // Button to submit PIN

    [Header("Door Settings")]
    public Animator doorAnimator1;       // First door Animator
    public Animator doorAnimator2;       // Second door Animator
    public string openTrigger = "Open";  // Name of trigger in Animator
    public string correctPin = "1942";   // The correct PIN code

    private bool doorsOpened = false;
    private bool playerInsideTrigger = false;

    private void Start()
    {
        // Disable the panel on start
        if (keypadPanel != null)
            keypadPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = "";

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitPin);

        // Ensure the trigger collider is set correctly
        if (keypadTrigger != null)
        {
            keypadTrigger.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("[KeypadDoorController] Keypad trigger collider not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react if this is the assigned trigger
        if (keypadTrigger == null || doorsOpened) return;

        if (other.gameObject == playerObject)
        {
            playerInsideTrigger = true;
            ShowKeypadPanel(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (keypadTrigger == null) return;

        if (other.gameObject == playerObject)
        {
            playerInsideTrigger = false;
            ShowKeypadPanel(false);
        }
    }

    private void ShowKeypadPanel(bool show)
    {
        if (keypadPanel != null)
            keypadPanel.SetActive(show);

        if (feedbackText != null)
            feedbackText.text = "";

        if (show)
        {
            pinInputField.text = "";
            pinInputField.ActivateInputField();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnSubmitPin()
    {
        string enteredPin = pinInputField.text.Trim();

        if (enteredPin == correctPin)
        {
            feedbackText.text = "Access Granted";
            OpenDoors();
            ShowKeypadPanel(false);
        }
        else
        {
            feedbackText.text = "Wrong answer. Try again.";
            pinInputField.text = "";
            pinInputField.ActivateInputField();
        }
    }

    private void OpenDoors()
    {
        if (doorAnimator1 != null)
            doorAnimator1.SetTrigger(openTrigger);

        if (doorAnimator2 != null)
            doorAnimator2.SetTrigger(openTrigger);

        doorsOpened = true;
    }
}
