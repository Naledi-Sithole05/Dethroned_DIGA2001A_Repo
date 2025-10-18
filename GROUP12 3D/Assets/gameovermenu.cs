 using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [Header("UI References")]
    public Button restartButton;
    public Button quitButton;
    public Text scoreText; // Optional: to display final score

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene"; // If you want restart to go to game

    void Start()
    {
        // Add button listeners
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
        
        // Optional: Display final score
        // DisplayFinalScore();
    }

    public void RestartGame()
    {
        // Option 1: Go to main menu
        SceneManager.LoadScene(mainMenuSceneName);
        
        // Option 2: Restart current game (if you have a game scene)
        // SceneManager.LoadScene(gameSceneName);
        
        // Option 3: Restart the current scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        // If in Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // In built application
        Application.Quit();
        #endif
    }

    // Optional: Method to display final score
    private void DisplayFinalScore()
    {
        // If you have a game manager that persists between scenes
        // if (GameManager.instance != null)
        // {
        //     scoreText.text = "Final Score: " + GameManager.instance.finalScore;
        // }
    }

    // Optional: Handle keyboard input
    void Update()
    {
        // Restart with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // Quit with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
}
