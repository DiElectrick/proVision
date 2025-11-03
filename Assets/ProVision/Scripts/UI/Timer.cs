using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private float roundTime = 60f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime;
    private bool isRunning = false;

    public delegate void TimerExpiredHandler();
    public event TimerExpiredHandler OnTimerExpired;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (isRunning && G.isRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                OnTimerExpired?.Invoke();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = (int)currentTime / 60;
        int seconds = (int)currentTime % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = roundTime;
        UpdateTimerDisplay();
    }

    public void ResetAndStartTimer()
    {
        ResetTimer();
        StartTimer();
    }

    public bool IsRunning => isRunning;
    public float CurrentTime => currentTime;
}