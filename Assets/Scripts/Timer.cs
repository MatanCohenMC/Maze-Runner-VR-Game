using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TimerText;
    [SerializeField] private float m_CurrentElapsedTime;

    private float m_ElapsedTime;
    private string m_CurrentTimeValue;
    private bool m_IsTimerRunning = false;

    private void Update()
    {
        if (m_IsTimerRunning)
        {
            m_ElapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        m_IsTimerRunning = true;
        m_ElapsedTime = 0f;
    }

    public void StopTimer()
    {
        m_IsTimerRunning = false;
        setCurrentTimerValue();
    }

    public void ResetTimer()
    {
        m_ElapsedTime = 0f;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(m_ElapsedTime / 60);
        int seconds = Mathf.FloorToInt(m_ElapsedTime % 60);
        m_TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void setCurrentTimerValue()
    {
        int minutes = Mathf.FloorToInt(m_ElapsedTime / 60);
        int seconds = Mathf.FloorToInt(m_ElapsedTime % 60);
        m_CurrentTimeValue = string.Format("{0:00}:{1:00}", minutes, seconds);
        m_CurrentElapsedTime = m_ElapsedTime;
    }

    public string GetCurrentTimerValue()
    {
        return m_CurrentTimeValue;
    }
    
    public float GetCurrentElapsedTime()
    {
        return m_CurrentElapsedTime;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }
}