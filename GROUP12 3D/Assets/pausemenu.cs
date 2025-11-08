  using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed"); // Add this line
            
            if (pauseMenu != null)
            {
                if (pauseMenu.activeInHierarchy)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
            else
            {
                Debug.LogError("PauseMenu reference is null!");
            }
        }
    }

    void Start()
    {
        if (pauseMenu == null)
        {
            Debug.LogError("PauseMenu reference not set in Inspector!");
            return;
        }
        
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        Debug.Log("Pausing game");
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home()
    {
        SceneManager.LoadScene("mainmenu");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        Debug.Log("Resuming game");
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1;
    }
}