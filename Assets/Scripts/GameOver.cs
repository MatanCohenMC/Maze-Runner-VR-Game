using TMPro;
using UnityEngine;

public enum eGameOver
{
    Win,
    Lose,
    Quit
}

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_GameOverText;
    [SerializeField] private TextMeshProUGUI m_TimerText;
    [SerializeField] private Timer m_Timer;

    public void DisplayGameOverMenu(eGameOver i_GameOverReason)
    {
        if(m_GameOverText != null)
        {
            if (i_GameOverReason == eGameOver.Win)
            {
                displayWonMessage();
            }
            else if (i_GameOverReason == eGameOver.Lose)
            {
                displayLostMessage();
            }
            else if (i_GameOverReason == eGameOver.Quit)
            {
                displayQuitMessage();
            }
        }
        else
        {
            Debug.Log("Gameover text is null");
        }

        if (m_TimerText != null)
        {
            displayTimer();
        }
        else
        {
            Debug.Log("Timer text is null");
        }
    }

    private void displayWonMessage()
    {
        m_GameOverText.text = "GOOD JOB!";
    } 
    
    private void displayLostMessage()
    {
        m_GameOverText.text = "Better luck next time!";
    }

    private void displayQuitMessage()
    {
        m_GameOverText.text = "Maybe try an easier level";
    }

    private void displayTimer()
    {
        m_TimerText.text = m_Timer.GetCurrentTimerValue();
    }
}
