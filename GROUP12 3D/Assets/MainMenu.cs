 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Reference to your settings panel GameObject
    public GameObject settingsPanel;
    
    // Reference to your help panel GameObject
    public GameObject helpPanel;

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void OpenSettings()
    {
        // Activate the settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Settings panel reference is missing!");
        }
    }

    public void OpenHelp()
    {
        // Activate the help panel
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Help panel reference is missing!");
        }
    }

    public void CloseSettings()
    {
        // Deactivate the settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void CloseHelp()
    {
        // Deactivate the help panel
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}