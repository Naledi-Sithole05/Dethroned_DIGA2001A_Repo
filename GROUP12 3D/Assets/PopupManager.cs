 using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    [Header("Popup UI Elements")]
    public GameObject popupPanel;
    public TextMeshProUGUI messageText;
    public Button closeButton;

    [Header("Messages")]
    public string patternMessage = "Watch carefully! The tiles will light up in a pattern. Remember the sequence and step on them in the correct order. This message will disappear automatically in 15 seconds.";

    [Header("Auto-close Settings")]
    public float autoCloseTime = 15f; // Time in seconds before auto-close
    public bool enableClickToClose = true; // Option to allow clicking to close early

    // Event to notify when popup is closed
    public System.Action OnPopupClosed;

    private bool popupIsActive = false;
    private Coroutine autoCloseCoroutine;

    private void Start()
    {
        // Hide popup at start
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePopup);
        }

        // Add click handler to the entire panel if enabled
        if (enableClickToClose)
        {
            SetupPanelClick();
        }
    }

    void SetupPanelClick()
    {
        if (popupPanel != null)
        {
            // Add EventTrigger component if not present
            EventTrigger trigger = popupPanel.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = popupPanel.AddComponent<EventTrigger>();
            }

            // Create new pointer click event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnPanelClicked((PointerEventData)data); });
            
            // Add the entry to the trigger
            trigger.triggers.Add(entry);

            Debug.Log("Panel click handler added");
        }
    }

    void OnPanelClicked(PointerEventData data)
    {
        Debug.Log("Panel clicked - closing popup");
        HidePopup();
    }

    public void ShowPatternMessage()
    {
        if (popupPanel != null && messageText != null)
        {
            messageText.text = patternMessage;
            popupPanel.SetActive(true);
            popupIsActive = true;
            
            // Pause game
            Time.timeScale = 0f;
            
            // Start auto-close coroutine
            if (autoCloseCoroutine != null)
                StopCoroutine(autoCloseCoroutine);
            autoCloseCoroutine = StartCoroutine(AutoClosePopup());
            
            Debug.Log($"Popup shown - Auto-closing in {autoCloseTime} seconds");
        }
    }

    IEnumerator AutoClosePopup()
    {
        // Wait for real seconds since time is scaled to 0
        yield return new WaitForSecondsRealtime(autoCloseTime);
        
        Debug.Log("Auto-close timer finished");
        HidePopup();
    }

    public void HidePopup()
    {
        if (!popupIsActive) return;
        
        Debug.Log("HidePopup called");
        
        // Stop the auto-close coroutine if it's running
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
            autoCloseCoroutine = null;
        }
        
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        
        popupIsActive = false;
        
        // Resume game
        Time.timeScale = 1f;

        // Notify that popup was closed
        OnPopupClosed?.Invoke();
        Debug.Log("Popup closed");
    }

    void Update()
    {
        // Optional: Keep escape key as manual close method
        if (popupIsActive && Input.GetKeyDown(KeyCode.Escape))
        {
            HidePopup();
        }
    }

    // Public method to change auto-close time if needed
    public void SetAutoCloseTime(float seconds)
    {
        autoCloseTime = seconds;
    }

    // Public method to toggle click-to-close
    public void SetClickToClose(bool enabled)
    {
        enableClickToClose = enabled;
    }
}