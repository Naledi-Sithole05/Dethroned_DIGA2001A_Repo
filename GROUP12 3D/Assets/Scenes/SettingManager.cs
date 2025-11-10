 using UnityEngine;
using UnityEngine.UI;

public class SimpleSettingsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeButton;
    
    [Header("Settings")]
    [SerializeField] private bool pauseGameWhenOpen = true;
    [SerializeField] private bool useEscapeKey = true;
    
    void Start()
    {
        InitializeSettings();
    }
    
    void InitializeSettings()
    {
        // Validate references
        if (settingsPanel == null)
        {
            Debug.LogError("SettingsPanel reference is missing! Please assign in Inspector.");
            return;
        }
        
        // Ensure settings panel is closed on start
        CloseSettings();
        
        // Setup settings button
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            Debug.Log("Settings button listener added");
        }
        else
        {
            Debug.LogError("SettingsButton reference is missing! Please assign in Inspector.");
        }
        
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
            Debug.Log("Close button listener added");
        }
        else
        {
            Debug.LogWarning("CloseButton reference is missing. Assign if you have a close button in settings panel.");
        }
        
        Debug.Log("SimpleSettingsManager initialized successfully");
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Debug.Log("Settings panel opened");
            
            // Pause game if enabled
            if (pauseGameWhenOpen)
            {
                Time.timeScale = 0f;
                Debug.Log("Game paused");
            }
        }
        else
        {
            Debug.LogError("Cannot open settings - SettingsPanel reference is null");
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("Settings panel closed");
            
            // Resume game if paused
            if (pauseGameWhenOpen)
            {
                Time.timeScale = 1f;
                Debug.Log("Game resumed");
            }
        }
    }
    
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            bool newState = !settingsPanel.activeSelf;
            settingsPanel.SetActive(newState);
            
            if (pauseGameWhenOpen)
            {
                Time.timeScale = newState ? 0f : 1f;
            }
            
            Debug.Log("Settings panel toggled: " + newState);
        }
    }
    
    void Update()
    {
        // Handle Escape key to close settings
        if (useEscapeKey && Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
        }
    }
    
    // Public methods for manual event binding
    public void OpenSettingsPublic() => OpenSettings();
    public void CloseSettingsPublic() => CloseSettings();
    public void ToggleSettingsPublic() => ToggleSettings();
    
    // Getters for external scripts
    public bool IsSettingsOpen()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }
    
    public GameObject GetSettingsPanel() => settingsPanel;
    
    void OnDestroy()
    {
        // Clean up listeners to prevent memory leaks
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OpenSettings);
        
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseSettings);
    }
}