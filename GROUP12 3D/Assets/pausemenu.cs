using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject helpPanel;
    
    [Header("Pause Button")]
    public GameObject pauseButton;
    
    private bool isPaused = false;

    void Start()
    {
        // Ensure all panels are closed at start
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);
        
        // Ensure pause button is visible
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    void Update()
    {
        // Optional: Add keyboard support (Escape key to pause/unpause)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Called by the Pause Button in your UI
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze game time
        
        // Show pause panel, hide pause button
        if (pausePanel != null) pausePanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
        
        // Close other panels
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);
    }

    // Called by the Resume Button
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume game time
        
        // Hide all panels, show pause button
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    // Called by the Settings Button
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            // Optionally hide other panels
            if (helpPanel != null) helpPanel.SetActive(false);
        }
    }

    // Called by the Help Button
    public void OpenHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            // Optionally hide other panels
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }
    }

    // Called by the Quit Button
    public void QuitGame()
    {
        // Resume time scale before quitting to avoid issues
        Time.timeScale = 1f;
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Optional: Back button functionality for settings/help panels
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void CloseHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
    }
}