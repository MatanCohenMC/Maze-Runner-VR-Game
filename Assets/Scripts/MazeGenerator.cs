using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.AI;

public enum eDirections
{
    RightDirections = 1, // PosX
    LeftDirections = 2, // NegX
    UpDirections = 3, // PosZ
    DownDirections = 4  // NegZ
}

public enum eWall
{
    RightWall = 0,
    LeftWall = 1,
    UpWall = 2,
    DownWall = 3,
    Amount = 4
}

public class MazeGenerator : MonoBehaviour
{
    private const int m_SpiderNavMeshAgentIndex = 1;
    [SerializeField] private MazeNode m_NodePrefab;
    [SerializeField] private MazeNode m_StartNodePrefab;
    [SerializeField] private MazeNode m_EndNodePrefab;
    [SerializeField] private List<MazeNode> m_ObstacleNodesPrefabs;
    [SerializeField] private int m_MazeYValue;

    private const int k_NodeDiameter = 5;
    private const int k_ObstaclesLevelEasy = 1;
    private const int k_ObstaclesLevelMedium = 2;
    private const int k_ObstaclesLevelHard = 3;
    List<string> m_NotIncludedLayersInNavMeshSurface = new List<string>{ "Enemy", "Interactable", "SliceableWood", "SliceableRock" };
    private List<MazeNode> m_Nodes;
    private List<MazeNode> m_ObstaclesNodes;
    private List<MazeNode> m_CurrentPath;
    private List<MazeNode> m_CompletedNodes;
    private List<MazeNode> m_LongestPath;
    private int m_MaxPathLength;

    public Transform PointToSpawnEnemies { get; private set; }
    public MazeNode StartNode { get; private set; }
    public MazeNode EndNode { get; private set; }

    public void Awake()
    {
        m_Nodes = new List<MazeNode>();
        m_ObstaclesNodes = new List<MazeNode>();
        m_CurrentPath = new List<MazeNode>();
        m_CompletedNodes = new List<MazeNode>();
        m_LongestPath = new List<MazeNode>();
        m_MaxPathLength = 0;
    }

    public void GenerateMazeInstant(GameLevel i_Level, int i_Rows, int i_Cols)
    {
        // Create all maze nodes
        createMazeNodes(i_Rows, i_Cols);

        // Choose starting node
        setStartingNode();
        m_CurrentPath.Add(StartNode); // Add the first node to the current path
        m_LongestPath.Add(StartNode); // Add the first node to the longest path
        m_MaxPathLength++;

        // Run DFS and create paths in the maze
        createPathsViaDFS(i_Rows, i_Cols);

        // Set the farthest node from the Start node to be the End node
        setEndNode();
        
        // Replace START and END node with the dedicated nodes
        replaceNodeWithStartNodePrefab();
        replaceNodeWithEndNodePrefab();

        // Adding obstacles to the maze
        addObstacles(i_Level);

        PointToSpawnEnemies = getStarterPointForEnemiesOnMaze();

        if (i_Level.Name == "Medium" || i_Level.Name == "Hard")
        {           
            // Adding enemies to the maze
            addNavMeshAgentToEnemies(i_Level);

            // Add navMesh
            buildNavMeshSurfacesThatOnMazeNodes();
        }
    }

    private Transform getStarterPointForEnemiesOnMaze()
    {
        MazeNode firstNode = null;
        int IndexThatMightIndicateTheFirstNode = 1;

        for (int i = IndexThatMightIndicateTheFirstNode; i < m_LongestPath.Count; i++)
        {
            if (m_LongestPath[i] != null && m_LongestPath[i].GetNodeState() == eNodeState.Normal)
            {
                firstNode = m_LongestPath[i];
                break;
            }
        }

        return firstNode.transform.Find("StarterPointOfEnemies");
    }

    private void buildNavMeshSurfacesThatOnMazeNodes()
    {
        NavMeshBaker navMeshBakerScript = GameObject.Find("NavMeshBaker").GetComponent<NavMeshBaker>();
        navMeshBakerScript.BuildNavMeshSurfaces(transform.Find("MazeNodes").transform);
    }

    private void setEndNode()
    {
        if(m_LongestPath.Count > 0)
        {
            EndNode = m_LongestPath[^1]; // Get the last node of the longest path
        }
    }

    private void setStartingNode()
    {
        int firstNodeIndex = Random.Range(0, m_Nodes.Count);

        StartNode = m_Nodes[firstNodeIndex];
        StartNode.SetState(eNodeState.Start);
    }

    private void createPathsViaDFS(int i_Rows, int i_Cols)
    {
        while(m_CompletedNodes.Count < m_Nodes.Count)
        {
            List<int> possibleNextNodes = new();
            List<int> possibleDirections = new();
            int currentNodeIndex = m_Nodes.IndexOf(m_CurrentPath[^1]); // Get the index of the last node in the current path
            int currentNodeX = currentNodeIndex / i_Rows;
            int currentNodeY = currentNodeIndex % i_Rows;

            checkPossibleDirections(i_Rows, i_Cols, currentNodeX, currentNodeIndex, possibleDirections, possibleNextNodes, currentNodeY);

            // Check if there is a possible direction to move to
            if(possibleDirections.Count > 0)
            {
                chooseDirection(possibleDirections, possibleNextNodes);
            }
            else
            {
                // check if the current path is the longest path so far
                if(m_CurrentPath.Count > m_MaxPathLength)
                {
                    setCurrentPathAsLongestPath();
                }

                m_CompletedNodes.Add(m_CurrentPath[^1]);
                m_CurrentPath.RemoveAt(m_CurrentPath.Count - 1);
            }
        }
    }

    private void chooseDirection(List<int> possibleDirections, List<int> possibleNextNodes)
    {
        int chosenDirection = Random.Range(0, possibleDirections.Count);
        MazeNode chosenNode = m_Nodes[possibleNextNodes[chosenDirection]];

        removeOnPathWalls(possibleDirections, chosenDirection, chosenNode);
        m_CurrentPath.Add(chosenNode);
    }

    private void checkPossibleDirections(
        int i_Rows,
        int i_Cols,
        int currentNodeX,
        int currentNodeIndex,
        List<int> possibleDirections,
        List<int> possibleNextNodes,
        int currentNodeY)
    {
        // Check if from the current node it's possible to go RIGHT
        if(currentNodeX < i_Cols - 1)
        {
            // Check node to the right of the current node
            checkRightNode(i_Rows, currentNodeIndex, possibleDirections, possibleNextNodes);
        }

        // Check if from the current node it's possible to go LEFT
        if(currentNodeX > 0)
        {
            // Check node to the left of the current node
            checkLeftNode(i_Rows, currentNodeIndex, possibleDirections, possibleNextNodes);
        }

        // Check if from the current node it's possible to go UP
        if(currentNodeY < i_Rows - 1)
        {
            // Check node above the current node
            checkUpNode(currentNodeIndex, possibleDirections, possibleNextNodes);
        }

        // Check if from the current node it's possible to go DOWN
        if(currentNodeY > 0)
        {
            // Check node below the current node
            checkDownNode(currentNodeIndex, possibleDirections, possibleNextNodes);
        }
    }

    private void checkDownNode(int currentNodeIndex, List<int> possibleDirections, List<int> possibleNextNodes)
    {
        if(!m_CompletedNodes.Contains(m_Nodes[currentNodeIndex - 1])
           && !m_CurrentPath.Contains(m_Nodes[currentNodeIndex - 1]))
        {
            possibleDirections.Add((int)eDirections.DownDirections);
            possibleNextNodes.Add(currentNodeIndex - 1);
        }
    }

    private void checkUpNode(int currentNodeIndex, List<int> possibleDirections, List<int> possibleNextNodes)
    {
        if(!m_CompletedNodes.Contains(m_Nodes[currentNodeIndex + 1])
           && !m_CurrentPath.Contains(m_Nodes[currentNodeIndex + 1]))
        {
            possibleDirections.Add((int)eDirections.UpDirections);
            possibleNextNodes.Add(currentNodeIndex + 1);
        }
    }

    private void checkLeftNode(int i_Rows, int currentNodeIndex, List<int> possibleDirections, List<int> possibleNextNodes)
    {
        if(!m_CompletedNodes.Contains(m_Nodes[currentNodeIndex - i_Rows])
           && !m_CurrentPath.Contains(m_Nodes[currentNodeIndex - i_Rows]))
        {
            possibleDirections.Add((int)eDirections.LeftDirections);
            possibleNextNodes.Add(currentNodeIndex - i_Rows);
        }
    }

    private void checkRightNode(int i_Rows, int currentNodeIndex, List<int> possibleDirections, List<int> possibleNextNodes)
    {
        if(!m_CompletedNodes.Contains(m_Nodes[currentNodeIndex + i_Rows])
           && !m_CurrentPath.Contains(m_Nodes[currentNodeIndex + i_Rows]))
        {
            possibleDirections.Add((int)eDirections.RightDirections);
            possibleNextNodes.Add(currentNodeIndex + i_Rows);
        }
    }

    private void setCurrentPathAsLongestPath()
    {
        m_LongestPath.Clear(); // Clear longestPath if it has any previous data
        m_LongestPath.AddRange(m_CurrentPath); // Copy elements from currentPath to longestPath
        m_MaxPathLength = m_CurrentPath.Count;
    }

    private void removeOnPathWalls(List<int> possibleDirections, int chosenDirection, MazeNode chosenNode)
    {
        switch(possibleDirections[chosenDirection])
        {
            case (int)eDirections.RightDirections:
                chosenNode.RemoveWall((int)eWall.LeftWall); // Remove the left wall of the chosen node
                m_CurrentPath[^1].RemoveWall((int)eWall.RightWall); // Remove the right wall of the current node
                break;
            case (int)eDirections.LeftDirections:
                chosenNode.RemoveWall((int)eWall.RightWall); // Remove the right wall of the chosen node
                m_CurrentPath[^1].RemoveWall((int)eWall.LeftWall); // Remove the left wall of the current node
                break;
            case (int)eDirections.UpDirections:
                chosenNode.RemoveWall((int)eWall.DownWall); // Remove the down wall of the chosen node
                m_CurrentPath[^1].RemoveWall((int)eWall.UpWall); // Remove the up wall of the current node
                break;
            case (int)eDirections.DownDirections:
                chosenNode.RemoveWall((int)eWall.UpWall); // Remove the up wall of the chosen node
                m_CurrentPath[^1].RemoveWall((int)eWall.DownWall); // Remove the down wall of the current node
                break;
        }
    }

    private void createMazeNodes(int i_Rows, int i_Cols)
    {
        Vector2Int mazeSize = new(i_Cols * k_NodeDiameter, i_Rows * k_NodeDiameter);

        for (int x = 0; x < mazeSize.x; x += 5)
        {
            for(int y = 0; y < mazeSize.y; y += 5)
            {
                Vector3 nodePos = new(x - (mazeSize.x / 2f), m_MazeYValue, y - (mazeSize.y / 2f));
                MazeNode newNode = Instantiate(m_NodePrefab, nodePos, Quaternion.identity, transform.Find("MazeNodes").transform);
                addNavMeshSurfaceToNode(newNode);
                m_Nodes.Add(newNode);
            }
        }
    }

    private void addNavMeshSurfaceToNode(MazeNode i_Node)
    {
        // add NavMeshSurface component to the Floor of mazeNode
        NavMeshSurface navMeshSurfaceComponent = i_Node.transform.Find("Floor").AddComponent<NavMeshSurface>();
        // change navMeshSurface's agentType to spider
        navMeshSurfaceComponent.agentTypeID = NavMesh.GetSettingsByIndex(m_SpiderNavMeshAgentIndex).agentTypeID;

        // Create a list of all layers that are not in m_NotIncludedLayersInNavMeshSurface list:
        var layerNames = new string[32];  // Unity supports 32 layers
        int index = 0;
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName) && !m_NotIncludedLayersInNavMeshSurface.Contains(layerName))
            {
                layerNames[index] = layerName;
                index++;
            }
        }

        // Set the layerMask to include all layers that in layerNames list
        navMeshSurfaceComponent.layerMask = LayerMask.GetMask(layerNames);
    }

    private void replaceNodeWithStartNodePrefab()
    {
        if (StartNode != null && m_StartNodePrefab != null)
        {
            // Create a new StartNode using the StartNodePrefab
            Vector3 nodePos = StartNode.transform.position;
            MazeNode newStartNode = Instantiate(m_StartNodePrefab, nodePos, Quaternion.identity, transform.Find("MazeNodes").transform);
            newStartNode.transform.Find("Floor").AddComponent<NavMeshSurface>().agentTypeID = NavMesh.GetSettingsByIndex(m_SpiderNavMeshAgentIndex).agentTypeID; // add NavMeshSurface component to the Floor of mazeNode

            // Remove the same walls from the new START node
            bool[] startNodeRemovedWalls = StartNode.GetRemovedWalls();
            for (int wallIndex = 0; wallIndex < (int)eWall.Amount; wallIndex++)
            {
                if(startNodeRemovedWalls[wallIndex])
                {
                    newStartNode.RemoveWall(wallIndex);
                }
            }

            // Remove the current node from the list of nodes and destroy it
            m_Nodes.Remove(StartNode);
            Destroy(StartNode.gameObject);

            // Update the StartNode reference to the new StartNode
            StartNode = newStartNode;

            // Set the state of the new StartNode
            StartNode.SetState(eNodeState.Start);
        }
    }

    private void replaceNodeWithEndNodePrefab()
    {
        if (EndNode != null && m_EndNodePrefab != null)
        {
            // Create a new EndNode using the EndNodePrefab
            Vector3 nodePos = EndNode.transform.position;
            MazeNode newEndNode = Instantiate(m_EndNodePrefab, nodePos, Quaternion.identity, transform.Find("MazeNodes").transform);
            newEndNode.transform.Find("Floor").AddComponent<NavMeshSurface>().agentTypeID = NavMesh.GetSettingsByIndex(m_SpiderNavMeshAgentIndex).agentTypeID; // add NavMeshSurface component to the Floor of mazeNode

            // Remove the same walls from the new END node
            bool[] endNodeRemovedWalls = EndNode.GetRemovedWalls();
            for (int wallIndex = 0; wallIndex < (int)eWall.Amount; wallIndex++)
            {
                if (endNodeRemovedWalls[wallIndex])
                {
                    newEndNode.RemoveWall(wallIndex);
                }
            }

            // Remove the current node from the list of nodes and destroy it
            m_Nodes.Remove(EndNode);
            Destroy(EndNode.gameObject);

            // Update the EndNode reference to the new EndNode
            EndNode = newEndNode;

            // Set the state of the new EndNode
            EndNode.SetState(eNodeState.End);
        }
    }

    private void replaceNodeWithObstacleNodePrefab(MazeNode i_ChosenObstacleNode)
    {
        int nodeIndex = Random.Range(0, m_ObstacleNodesPrefabs.Count);
        MazeNode obstacleNodePrefab = m_ObstacleNodesPrefabs[nodeIndex];
        // obstacleNodePrefab = m_ObstacleNodesPrefabs[0]; // For testing obstacle1 // 
        // obstacleNodePrefab = m_ObstacleNodesPrefabs[1]; // For testing obstacle2 // 
        // obstacleNodePrefab = m_ObstacleNodesPrefabs[2]; // For testing obstacle3 //
        // obstacleNodePrefab = m_ObstacleNodesPrefabs[3]; // For testing obstacle4 //

        if (obstacleNodePrefab != null)
        {
            // Create a new EndNode using the EndNodePrefab
            Vector3 chosenNodePos = i_ChosenObstacleNode.transform.position;
            MazeNode obstacleNode = Instantiate(obstacleNodePrefab, chosenNodePos, Quaternion.identity, transform);

            // Remove the same walls from the new END node
            bool[] chosenObstacleNodeRemovedWalls = i_ChosenObstacleNode.GetRemovedWalls();
            for (int wallIndex = 0; wallIndex < (int)eWall.Amount; wallIndex++)
            {
                if (chosenObstacleNodeRemovedWalls[wallIndex])
                {
                    obstacleNode.RemoveWall(wallIndex);
                }
            }

            // Remove the current node from the list of nodes and destroy it
            m_Nodes.Remove(i_ChosenObstacleNode);
            m_LongestPath.Remove(i_ChosenObstacleNode);
            Destroy(i_ChosenObstacleNode.gameObject);

            // Add the new obstacle node to the obstacle node list
            m_ObstaclesNodes.Add(obstacleNode);

            // Set the state of the new EndNode
            obstacleNode.SetState(eNodeState.Obstacle);
        }
    }

    private void addObstacles(GameLevel i_Level)
    {
        if (i_Level.Name == "Easy")
        {
            addObstaclesToMazePath(k_ObstaclesLevelEasy);
        }
        else if (i_Level.Name == "Medium")
        {
            addObstaclesToMazePath(k_ObstaclesLevelMedium);
        }
        else if (i_Level.Name == "Hard")
        {
            addObstaclesToMazePath(k_ObstaclesLevelHard);
        }
        else
        {
            Debug.Log("No obstacles are added at level: " + i_Level);
        }
    }

    private void addObstaclesToMazePath(int i_ObstaclesAmount)
    {
        for(int i = 0; i < i_ObstaclesAmount; i++)
        {
            int nodeIndex;

            do
            {
                // Choose random normal node on the longest path that:
                nodeIndex = Random.Range(1, m_LongestPath.Count - 1);
            }
            while (m_LongestPath[nodeIndex].GetNodeState() != eNodeState.Normal);

            // Replace that node with a random obstacle node
            replaceNodeWithObstacleNodePrefab(m_LongestPath[nodeIndex]);
        }
    }

    private void addNavMeshAgentToEnemies(GameLevel i_Level)
    {
        EnemiesSpawnerManager enemiesSpawnerManagerScript = GameObject.Find("Enemies And Obsticles Manager").GetComponentInChildren<EnemiesSpawnerManager>();        
        NavMeshBaker navMeshBakerScript = GameObject.Find("NavMeshBaker").GetComponent<NavMeshBaker>();

        if (i_Level.Name == "Medium")
        {
            navMeshBakerScript.AddNavMeshAgent(enemiesSpawnerManagerScript.EasyEnemiesToSpawnStorage, PointToSpawnEnemies);
        }
        else if (i_Level.Name == "Hard")
        {
            navMeshBakerScript.AddNavMeshAgent(enemiesSpawnerManagerScript.AdvancedEnemiesToSpawnStorage, PointToSpawnEnemies);
        }
        else
        {
            Debug.Log("No enemies are added at level: " + i_Level);
        }
    }

    public void DeleteMaze()
    {
        // Destroy all maze nodes
        foreach (MazeNode node in m_Nodes)
        {
            Destroy(node.gameObject);
        }

        foreach (MazeNode node in m_ObstaclesNodes)
        {
            Destroy(node.gameObject);
        }

        // Destroy the START and END nodes
        if(StartNode != null && EndNode != null)
        {
            Destroy(StartNode.gameObject);
            Destroy(EndNode.gameObject);
        }
        
        // Clear lists and references
        m_Nodes.Clear();
        m_CurrentPath.Clear();
        m_CompletedNodes.Clear();
        m_ObstaclesNodes.Clear();
        m_LongestPath.Clear();
        m_MaxPathLength = 0;

        // Clear StartNode and EndNode references
        StartNode = null;
        EndNode = null;
    }
}