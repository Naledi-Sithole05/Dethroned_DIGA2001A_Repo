  using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.SceneManagement;

public class VideoOpeningScene : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    
    [Header("UI Elements")]
    public Button playButton; // Button to go to next scene
    public Button rewatchButton; // Button to replay the video
    public Button skipButton; // Button to skip the video
    public GameObject buttonsParent; // Parent object for play/rewatch buttons
    public GameObject skipButtonObject; // Separate reference for skip button
    
    [Header("Scene Settings")]
    public string nextSceneName = "SampleScene"; // Change this to your actual scene name
    
    void Start()
    {
        // Set up video player
        videoPlayer.targetTexture = new RenderTexture((int)videoDisplay.rectTransform.rect.width, 
                                                     (int)videoDisplay.rectTransform.rect.height, 24);
        videoDisplay.texture = videoPlayer.targetTexture;
        
        // Hide end buttons initially, BUT SHOW SKIP BUTTON
        buttonsParent.SetActive(false);
        skipButtonObject.SetActive(true); // Skip button is visible from the start
        
        // Add event for when video finishes
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // Set up button click handlers
        playButton.onClick.AddListener(OnPlayButtonClicked);
        rewatchButton.onClick.AddListener(OnRewatchButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
        
        // Start playing video
        videoPlayer.Play();
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        // Show buttons when video ends
        ShowEndButtons();
    }
    
    // Skip button handler - skip directly to end buttons
    public void OnSkipButtonClicked()
    {
        // Stop the video
        videoPlayer.Stop();
        
        // Show the end buttons (play and rewatch) and hide skip button
        ShowEndButtons();
    }
    
    // Helper method to show end buttons
    private void ShowEndButtons()
    {
        buttonsParent.SetActive(true);
        skipButtonObject.SetActive(false); // Hide skip button when video ends
        StartCoroutine(FadeInButtons());
    }
    
    // Play button handler - go to next scene
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    
    // Rewatch button handler - replay the video
    public void OnRewatchButtonClicked()
    {
        // Hide end buttons and show skip button again
        buttonsParent.SetActive(false);
        skipButtonObject.SetActive(true);
        
        // Rewind and play video again
        videoPlayer.Stop();
        videoPlayer.Play();
    }
    
    // Optional: Smooth buttons appearance
    private IEnumerator FadeInButtons()
    {
        CanvasGroup buttonsCanvasGroup = buttonsParent.GetComponent<CanvasGroup>();
        if (buttonsCanvasGroup == null)
            buttonsCanvasGroup = buttonsParent.AddComponent<CanvasGroup>();
            
        buttonsCanvasGroup.alpha = 0f;
        
        float duration = 1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            buttonsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        buttonsCanvasGroup.alpha = 1f;
    }
    
    void OnDestroy()
    {
        // Clean up events
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
            
        // Remove button listeners
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        rewatchButton.onClick.RemoveListener(OnRewatchButtonClicked);
        skipButton.onClick.RemoveListener(OnSkipButtonClicked);
    }
}