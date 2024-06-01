using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private Timer m_Timer;

    private ScoreData m_EasyScoresData;
    private ScoreData m_MediumScoresData;
    private ScoreData m_HardScoresData;
    private float m_PlayerScore;
    public GameObject m_EasyLeaderBoardContent;
    public GameObject m_MediumLeaderBoardContent;
    public GameObject m_HardLeaderBoardContent;

    void Awake()
    {
        m_EasyScoresData = new ScoreData();
        m_MediumScoresData = new ScoreData();
        m_HardScoresData = new ScoreData();
    }

    private void Start()
    {
        // Subscribe the SetupPlayerScore method to the OnGameSetup event in the GameManager instance.
        //GameManager.Instance.OnGameSetup += SetupPlayerScore;
        resetPlayerScore();
    }

    public void SetPlayerScore()
    {
        m_PlayerScore = m_Timer.GetCurrentElapsedTime();
    }

    public float GetPlayerScore()
    {
        return m_PlayerScore;
    }

    private void resetPlayerScore()
    {
        m_PlayerScore = 0;
    }

    // This method returns a sorted leaderboard - the lowest m_ElapsedTime values, will be at the top of the leaderboard.
    public IEnumerable<Score> SortedHighScoreLeaderBoard(ScoreData i_ScoresData)
    {
        return i_ScoresData.m_Scores.OrderBy(x => x.m_ElapsedTime);
    }

    // this method adds a Score to leaderBoard
    public void AddScoreToLeaderBoard(Score i_Score, GameLevel i_CurrentGameLevel)
    {
        if(i_CurrentGameLevel.Name == "Easy")
        {
            m_EasyScoresData.m_Scores.Add(i_Score);
            this.GetComponent<LeaderboardScore>().PresentSortedLeaderBoard(m_EasyScoresData, i_CurrentGameLevel);
        }
        else if(i_CurrentGameLevel.Name == "Medium")
        {
            m_MediumScoresData.m_Scores.Add(i_Score);
            this.GetComponent<LeaderboardScore>().PresentSortedLeaderBoard(m_MediumScoresData, i_CurrentGameLevel);
        }
        else if(i_CurrentGameLevel.Name == "Hard")
        {
            m_HardScoresData.m_Scores.Add(i_Score);
            this.GetComponent<LeaderboardScore>().PresentSortedLeaderBoard(m_HardScoresData, i_CurrentGameLevel);
        }
    }

    // this method resets score array of the LeaderBoard
    public void ResetScoreLeaderBoard()
    {
        m_EasyScoresData.m_Scores?.Clear();
        Debug.Log("Score Data was cleared");
    }
}

[Serializable]
public class Score
{
    public string m_Name;
    public float m_ElapsedTime;

    public Score(string name, float elapsedTime)
    {
        this.m_Name = name;
        this.m_ElapsedTime = elapsedTime;
    }
}

[Serializable]
public class ScoreData
{
    public List<Score> m_Scores;

    public ScoreData()
    {
        m_Scores = new List<Score>();
    }
}