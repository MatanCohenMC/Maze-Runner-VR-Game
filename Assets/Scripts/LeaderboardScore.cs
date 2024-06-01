using System.Linq;
using UnityEngine;

public class LeaderboardScore : MonoBehaviour
{
    [SerializeField] private LeaderboardRow m_Row;

    private const int k_MaximumPlayersOnLeaderBoard = 3;
    private LeaderboardManager m_LeaderboardManager;
    private GameObject m_EasyLeaderboardContent;
    private GameObject m_MediumLeaderboardContent;
    private GameObject m_HardLeaderboardContent;

    void Awake()
    {
        if(m_Row == null)
        {
            Debug.Log("m_Row is null");
        }
    }

    void Start()
    {
        getMembersComponents();
    }

    private void getMembersComponents()
    {
        m_LeaderboardManager = GameObject.Find("Leaderboard Manager").GetComponent<LeaderboardManager> ();
        if (m_LeaderboardManager == null)
        {
            Debug.Log("m_LeaderboardManager is null");
        }

        m_EasyLeaderboardContent = m_LeaderboardManager.m_EasyLeaderBoardContent;
        if (m_EasyLeaderboardContent == null)
        {
            Debug.Log("m_LeaderboardContent is null");
        }

        m_MediumLeaderboardContent = m_LeaderboardManager.m_MediumLeaderBoardContent;
        if (m_MediumLeaderboardContent == null)
        {
            Debug.Log("m_LeaderboardContent is null");
        }

        m_HardLeaderboardContent = m_LeaderboardManager.m_HardLeaderBoardContent;
        if (m_HardLeaderboardContent == null)
        {
            Debug.Log("m_LeaderboardContent is null");
        }
    }

    public void PresentSortedLeaderBoard(ScoreData i_ScoresData, GameLevel i_CurrentGameLevel)
    {
        removeContentRows(i_CurrentGameLevel);
        Score[] scores = m_LeaderboardManager.SortedHighScoreLeaderBoard(i_ScoresData).ToArray();
        addContentRows(scores, i_CurrentGameLevel);

        Debug.Log("Presented the leaderboard");
    }

    private void addContentRows(Score[] scores, GameLevel i_CurrentGameLevel)
    {
        for (int i = 0; i < scores.Length && i < k_MaximumPlayersOnLeaderBoard; i++)
        {
            LeaderboardRow row = i_CurrentGameLevel.Name switch
                {
                    "Easy" => Instantiate(m_Row, m_EasyLeaderboardContent.transform).GetComponent<LeaderboardRow>(),
                    "Medium" => Instantiate(m_Row, m_MediumLeaderboardContent.transform).GetComponent<LeaderboardRow>(),
                    "Hard" => Instantiate(m_Row, m_HardLeaderboardContent.transform).GetComponent<LeaderboardRow>(),
                    _ => new LeaderboardRow()
                };

            row.Rank.text = (i + 1).ToString();
            row.Name.text = scores[i].m_Name;

            int minutes = Mathf.FloorToInt(scores[i].m_ElapsedTime / 60);
            int seconds = Mathf.FloorToInt(scores[i].m_ElapsedTime % 60);
            row.Time.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            Debug.Log($"Added row:{row.Rank.text},{row.Name.text},{row.Time.text} to {i_CurrentGameLevel.Name} leaderboard");
        }
    }

    public void ResetLeaderboard()
    {
        GameObject.Find("Leaderboard Manager").GetComponent< LeaderboardManager > ()?.ResetScoreLeaderBoard();
        //removeContentRows();
    }

    private void removeContentRows(GameLevel i_CurrentGameLevel)
    {
        if(i_CurrentGameLevel.Name == "Easy")
        {
            foreach (Transform child in m_EasyLeaderboardContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else if (i_CurrentGameLevel.Name == "Medium")
        {
            foreach (Transform child in m_MediumLeaderboardContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else if (i_CurrentGameLevel.Name == "Hard")
        {
            foreach (Transform child in m_HardLeaderboardContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

    }
}