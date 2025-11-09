 using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PopupManager : MonoBehaviour
{
    [Header("Popup UI Elements")]
    public GameObject popupPanel;
    public TextMeshProUGUI messageText;
    public Button closeButton; // We'll keep this but make the whole panel clickable

    [Header("Messages")]
    public string patternMessage = "Watch carefully! The tiles will light up in a pattern. Remember the sequence and step on them in the correct order. Click anywhere to close this message, then press SPACE to start when you're ready.";

    // Event to notify when popup is closed
    public System.Action OnPopupClosed;

    private void Start()
    {
        // Hide popup at start
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // Setup close button (optional now, but keep for consistency)
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePopup);
        }

        // Add click handler to the entire panel
        SetupPanelClick();
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
            entry.callback.AddListener((data) => { OnPanelClicked(); });
            
            // Add the entry to the trigger
            trigger.triggers.Add(entry);

            Debug.Log("Panel click handler added");
        }
    }

    void OnPanelClicked()
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
            
            // Pause game
            Time.timeScale = 0f;
            
            Debug.Log("Popup shown - Click anywhere to close");
        }
    }

    public void HidePopup()
    {
        Debug.Log("HidePopup called");
        
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        
        // Resume game
        Time.timeScale = 1f;

        // Notify that popup was closed
        OnPopupClosed?.Invoke();
        Debug.Log("Popup closed - Press SPACE to start pattern");
    }

    void Update()
    {
        // Allow closing popup with any key or mouse click (backup method)
        if (popupPanel != null && popupPanel.activeInHierarchy && 
            (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            // Don't close if the click was on the button (to avoid double-trigger)
            if (!IsPointerOverButton())
            {
                HidePopup();
            }
        }
    }

    bool IsPointerOverButton()
    {
        // Check if the pointer is over a UI element (like the close button)
        if (EventSystem.current == null) return false;
        
        return EventSystem.current.IsPointerOverGameObject();
    }
}