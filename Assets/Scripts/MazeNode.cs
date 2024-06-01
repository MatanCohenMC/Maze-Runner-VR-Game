using UnityEngine;

public enum eNodeState
{
    Available,
    Current,
    Completed,
    Start,
    End,
    Obstacle,
    Normal
}

public class MazeNode : MonoBehaviour
{
    [SerializeField] private GameObject[] m_Walls;
    [SerializeField] private MeshRenderer m_Floor;

    private bool[] m_RemovedWalls = new bool[4]; // [false,false,false,false]
    private eNodeState m_NodeState = eNodeState.Normal;

    public void SetState(eNodeState i_State)
    {
        switch(i_State)
        {
            case eNodeState.Available:
                m_Floor.material.color = Color.white; break;
            case eNodeState.Current:
                m_Floor.material.color = Color.yellow; break;
            case eNodeState.Completed:
                m_Floor.material.color = Color.blue; break;
            case eNodeState.Start:
                m_NodeState = i_State;
                break;
            case eNodeState.End:
                m_NodeState = i_State;
                break;
            case eNodeState.Obstacle:
                m_NodeState = i_State;
                break;
        }
    }

    public eNodeState GetNodeState()
    {
        return m_NodeState;
    }

    public bool[] GetRemovedWalls()
    {
        return m_RemovedWalls;
    }

    public void RemoveWall(int i_WallToRemove)
    {
        m_RemovedWalls[i_WallToRemove] = true;
        m_Walls?[i_WallToRemove].gameObject.SetActive(false);
    }
}