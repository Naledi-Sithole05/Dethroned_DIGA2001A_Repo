 using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject helpPanel;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Pause game time
    }

    public void Home()
    {
        Time.timeScale = 1f; // Resume time before loading menu
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Resume time before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        // Optional: hide pause menu when settings open
        // pauseMenu.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        // Optional: show pause menu again when settings close
        // pauseMenu.SetActive(true);
    }

    public void OpenHelp()
    {
        helpPanel.SetActive(true);
        // Optional: hide pause menu when help opens
        // pauseMenu.SetActive(false);
    }

    public void CloseHelp()
    {
        helpPanel.SetActive(false);
        // Optional: show pause menu again when help closes
        // pauseMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}