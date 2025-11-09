 using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundToggle : MonoBehaviour
{
    [Header("UI References")]
    public Button soundOnButton;
    public Button soundOffButton;
    
    [Header("Audio Settings")]
    public AudioListener audioListener; // Optional: for global audio
    public AudioSource[] audioSources; // For specific audio sources
    
    private bool isSoundOn = true;

    void Start()
    {
        // Ensure we have button references
        if (soundOnButton == null || soundOffButton == null)
        {
            Debug.LogError("Sound buttons are not assigned!");
            return;
        }

        // Load saved sound setting first
        LoadSoundSetting();
        
        // Then setup buttons
        SetupButtons();
        
        // Force update the button states
        UpdateButtonStates();
    }

    void OnEnable()
    {
        // This automatically gets called when the GameObject becomes active
        // (when the settings panel opens)
        RefreshSoundButtons();
    }

    private void SetupButtons()
    {
        // Remove existing listeners to avoid duplicates
        soundOnButton.onClick.RemoveAllListeners();
        soundOffButton.onClick.RemoveAllListeners();
        
        // Add click listeners
        soundOnButton.onClick.AddListener(ToggleSound);
        soundOffButton.onClick.AddListener(ToggleSound);
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        ApplySoundSetting();
        UpdateButtonStates();
        SaveSoundSetting();
        
        Debug.Log($"Sound toggled to: {(isSoundOn ? "ON" : "OFF")}");
    }

    private void ApplySoundSetting()
    {
        // Method 1: Control AudioListener (global audio)
        if (audioListener != null)
        {
            audioListener.enabled = isSoundOn;
        }
        else
        {
            AudioListener.volume = isSoundOn ? 1f : 0f;
        }
        
        // Method 2: Control specific AudioSources
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource != null)
            {
                audioSource.mute = !isSoundOn;
            }
        }
    }

    private void UpdateButtonStates()
    {
        // Show/hide buttons based on sound state
        soundOnButton.gameObject.SetActive(isSoundOn);
        soundOffButton.gameObject.SetActive(!isSoundOn);
        
        // Debug information
        Debug.Log($"UpdateButtonStates: Sound={isSoundOn}, " +
                 $"OnButton.active={soundOnButton.gameObject.activeInHierarchy}, " +
                 $"OffButton.active={soundOffButton.gameObject.activeInHierarchy}");
    }

    // Call this when the settings panel opens or when you need to refresh
    public void RefreshSoundButtons()
    {
        // Small delay to ensure everything is initialized
        StartCoroutine(RefreshAfterFrame());
    }
    
    private IEnumerator RefreshAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        
        // Reload settings to ensure we have the current state
        LoadSoundSetting();
        UpdateButtonStates();
        
        Debug.Log("Sound buttons refreshed. Current state: " + (isSoundOn ? "ON" : "OFF"));
    }

    private void SaveSoundSetting()
    {
        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSoundSetting()
    {
        if (PlayerPrefs.HasKey("SoundEnabled"))
        {
            isSoundOn = PlayerPrefs.GetInt("SoundEnabled") == 1;
        }
        else
        {
            // Default to sound ON
            isSoundOn = true;
            SaveSoundSetting();
        }
        ApplySoundSetting();
    }

    // Public methods for external control
    public void TurnSoundOn()
    {
        SetSound(true);
    }
    
    public void TurnSoundOff()
    {
        SetSound(false);
    }

    public void SetSound(bool enabled)
    {
        isSoundOn = enabled;
        ApplySoundSetting();
        UpdateButtonStates();
        SaveSoundSetting();
    }

    public bool IsSoundOn()
    {
        return isSoundOn;
    }

    // Optional: Add this if you want to manually trigger refresh from other scripts
    [ContextMenu("Force Refresh Sound Buttons")]
    private void ForceRefresh()
    {
        RefreshSoundButtons();
    }
}
