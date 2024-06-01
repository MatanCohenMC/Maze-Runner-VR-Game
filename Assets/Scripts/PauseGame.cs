using UnityEngine;
using TMPro;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_PauseButtonText;

    private bool m_IsPaused = false;

    public void PauseButtonPressed()
    {
        if (m_IsPaused)
        {
            ResumeGame();
        }
        else
        {
            Pause();
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        m_IsPaused = true;
        m_PauseButtonText.text = "Resume";
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        m_IsPaused = false;
        m_PauseButtonText.text = "Pause";
    }
}