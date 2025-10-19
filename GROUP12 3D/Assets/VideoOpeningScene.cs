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
    public GameObject buttonsParent; // Parent object for both buttons
    
    [Header("Scene Settings")]
    public string nextSceneName = "SampleScene"; // Change this to your actual scene name
    
    void Start()
    {
        // Set up video player
        videoPlayer.targetTexture = new RenderTexture((int)videoDisplay.rectTransform.rect.width, 
                                                     (int)videoDisplay.rectTransform.rect.height, 24);
        videoDisplay.texture = videoPlayer.targetTexture;
        
        // Hide buttons initially
        buttonsParent.SetActive(false);
        
        // Add event for when video finishes
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // Set up button click handlers
        playButton.onClick.AddListener(OnPlayButtonClicked);
        rewatchButton.onClick.AddListener(OnRewatchButtonClicked);
        
        // Start playing video
        videoPlayer.Play();
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        // Show buttons when video ends
        buttonsParent.SetActive(true);
        
        // Optional: Add fade-in animation
        StartCoroutine(FadeInButtons());
    }
    
    // Play button handler - go to next scene
    public void OnPlayButtonClicked()
    {
        // Load next scene - MAKE SURE THIS SCENE NAME EXISTS IN YOUR BUILD SETTINGS
        SceneManager.LoadScene(nextSceneName);
    }
    
    // Rewatch button handler - replay the video
    public void OnRewatchButtonClicked()
    {
        // Hide buttons
        buttonsParent.SetActive(false);
        
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
    }
}