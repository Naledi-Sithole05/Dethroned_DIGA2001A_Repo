using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime = 10f;
    [SerializeField] private GuardAI guardAI; // drag the existing GuardAI object here

    private bool guardActivated = false;

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else if (!guardActivated)
        {
            remainingTime = 0;
            UpdateTimerDisplay();
            ActivateGuard();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void ActivateGuard()
    {
        guardActivated = true;
        if (guardAI != null)
        {
            guardAI.ActivateGuard();
        }
        else
        {
            Debug.LogError("TimerController: GuardAI not assigned!");
        }
    }
}
