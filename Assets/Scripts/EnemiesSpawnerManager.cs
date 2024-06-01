using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum eEnemiesSettings
{
    MediumMaxEnemyCount = 2,
    HardMaxEnemyCount = 3,
    MediumSecondsToWaitBetweenSpawningEnemies = 45,
    HardSecondsToWaitBetweenSpawningEnemies = 30
}

public class EnemiesSpawnerManager : MonoBehaviour
{
    [SerializeField] private int m_EnemyDuplicationCount = 3;
    [SerializeField] private List<GameObject> m_EasyEnemiesToSpawn;
    [SerializeField] private List<GameObject> m_AdvancedEnemiesToSpawn;
    [SerializeField] private int m_MaxEnemyCount;
    [SerializeField] private float m_SecondsToWaitBetweenSpawningEnemies;

    private int m_CurrentEnemyCount;
    private float m_NextSpawnTime;
    private List<GameObject> m_EasyEnemiesToSpawnStorage;
    private List<GameObject> m_AdvancedEnemiesToSpawnStorage;
    private MazeManager m_MazeManager;
    private Transform m_PointToSpawnEnemies;
    private bool m_IsFunctionRunning = false;
    private bool m_IsUpdating = false;
    public List<GameObject> EasyEnemiesToSpawnStorage { get { return m_EasyEnemiesToSpawnStorage; } }
    public List<GameObject> AdvancedEnemiesToSpawnStorage { get { return m_AdvancedEnemiesToSpawnStorage; } }

    void Start()
    {
        m_MazeManager = GameObject.Find("Maze Manager").GetComponent<MazeManager>();
        GameManager.OnPlayModeStart += InitializeSpawnSettings;
        GameManager.OnPlayModeStart += SetEnemiesSpawnerSettings;
        GameManager.OnPlayModeEnd += DeactivateEasyOrAdvancedEnemies;

        m_EasyEnemiesToSpawnStorage = new List<GameObject>();
        m_AdvancedEnemiesToSpawnStorage = new List<GameObject>();
        setStorageOfEasyEnemiesToSpawn();
        setStorageOfAdvancedEnemiesToSpawn();
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == eGameState.Playing)
        {
            // Update active enemies's position on maze
            switch (m_MazeManager.CurrentGameLevel.Name)
            {
                case "Medium":
                    if (!m_IsUpdating)
                    {
                        m_IsUpdating = true;
                        updateEnemiesAgentDestinationToMainCamera(m_EasyEnemiesToSpawnStorage);
                        m_IsUpdating = false;
                    }
                    if (!m_IsFunctionRunning && m_CurrentEnemyCount < m_MaxEnemyCount && Time.time > m_NextSpawnTime)
                    {
                        m_IsFunctionRunning = true;
                        // Spawn more enemies
                        SpawnEnemyOnStartMaze(m_EasyEnemiesToSpawnStorage);
                        m_IsFunctionRunning = false;
                    }
                    break;

                case "Hard":
                    if (!m_IsUpdating)
                    {
                        m_IsUpdating = true;
                        updateEnemiesAgentDestinationToMainCamera(m_AdvancedEnemiesToSpawnStorage);
                        m_IsUpdating = false;
                    }                 
                    if (!m_IsFunctionRunning && m_CurrentEnemyCount < m_MaxEnemyCount && Time.time > m_NextSpawnTime)
                    {
                        m_IsFunctionRunning = true;
                        // Spawn more enemies                     
                        SpawnEnemyOnStartMaze(m_AdvancedEnemiesToSpawnStorage);                  
                        m_IsFunctionRunning = false;
                    }
                    break;
            }                  
        }                 
    }

    public void DeactivateEasyOrAdvancedEnemies()
    {
        switch (m_MazeManager.CurrentGameLevel.Name)
        {
            case "Medium":
                deactivateEnemies(m_EasyEnemiesToSpawnStorage);
                break;

            case "Hard":
                deactivateEnemies(m_AdvancedEnemiesToSpawnStorage);
                break;
        }
    }

    public void InitializeSpawnSettings()
    {
        m_NextSpawnTime = Time.time + m_SecondsToWaitBetweenSpawningEnemies;
        m_CurrentEnemyCount = 0;
        m_PointToSpawnEnemies = m_MazeManager.transform.Find("Maze Generator").GetComponent<MazeGenerator>().PointToSpawnEnemies;
        m_IsFunctionRunning = false;
        m_IsUpdating = false;
    }

    private void deactivateEnemies(List<GameObject> i_Enemies)
    {
        foreach (GameObject enemy in i_Enemies)
        {
            if (enemy.activeSelf)
            {
                enemy.SetActive(false);
            }
        }
    }

    public void SetEnemiesSpawnerSettings()
    {
        switch (m_MazeManager.CurrentGameLevel.Name)
        {
            case "Medium":
                m_MaxEnemyCount = ((int)eEnemiesSettings.MediumMaxEnemyCount);
                m_SecondsToWaitBetweenSpawningEnemies = ((int)eEnemiesSettings.MediumSecondsToWaitBetweenSpawningEnemies);
                break;

            case "Hard":
                m_MaxEnemyCount = ((int)eEnemiesSettings.HardMaxEnemyCount);
                m_SecondsToWaitBetweenSpawningEnemies = ((int)eEnemiesSettings.HardSecondsToWaitBetweenSpawningEnemies);
                break;
        }
       
    }

    public void SpawnEnemyOnStartMaze(List<GameObject> i_EnemyStorage)
    {
        // Get a random integer between 0 (inclusive) and i_EnemyStorage.Count (exclusive).
        int randomIndex = Random.Range(0, i_EnemyStorage.Count);
        if (!i_EnemyStorage[randomIndex].activeSelf) // if enemy is not active
        {
            initEnemySettings(i_EnemyStorage[randomIndex]);
            i_EnemyStorage[randomIndex].SetActive(true);
            m_CurrentEnemyCount++;
            m_NextSpawnTime = Time.time + m_SecondsToWaitBetweenSpawningEnemies;
        }
    }

    public void MakeEnemyDead(GameObject enemy)
    {
        enemy.GetComponent<Animator>().SetBool("isDead", true);

        NavMeshAgent navMeshAgentComponent = enemy.GetComponent<NavMeshAgent>();
        if(navMeshAgentComponent != null)
        {
            // stop enemy to run after the player
            navMeshAgentComponent.isStopped = true;
        }
    }

    public void DecreaseEnemyCountByOneAndUpdateSpawnTimeNextEnemy()
    {
        m_CurrentEnemyCount--;
        m_NextSpawnTime = Time.time + m_SecondsToWaitBetweenSpawningEnemies;
    }

    private void initEnemySettings(GameObject enemy)
    {
        enemy.transform.SetPositionAndRotation(m_PointToSpawnEnemies.position, m_PointToSpawnEnemies.rotation);
        enemy.GetComponent<HealthManager>().ResetHealth();
    }

    private void setStorageOfEnemiesToSpawn(List<GameObject> i_EnemyToSpawnList, List<GameObject> i_EnemyToSpawnListStorage, string i_NameOfStorage)
    {
         // Create a new empty GameObject
        GameObject StorageOfEnemiesToSpawn = new GameObject(i_NameOfStorage);

        // Find the GameObject with the name "EnemiesSpawner"
        GameObject enemiesSpawner = GameObject.Find("EnemiesSpawner");

        // Check if the GameObject was not found
        if (enemiesSpawner == null)
        {
            Debug.LogWarning("No GameObject with the name 'EnemiesSpawner' found.");
        }

        // Set the parent of the new GameObject to EnemiesSpawner
        StorageOfEnemiesToSpawn.transform.SetParent(enemiesSpawner.transform);

        foreach (GameObject enemy in i_EnemyToSpawnList)
        {
            for (int i = 0; i < m_EnemyDuplicationCount; i++)
            {
                GameObject duplicatedEnemy = Instantiate(enemy);
                duplicatedEnemy.SetActive(false);
                duplicatedEnemy.transform.SetParent(StorageOfEnemiesToSpawn.transform);
                i_EnemyToSpawnListStorage.Add(duplicatedEnemy);
            }
        }
    }

    private void updateEnemiesAgentDestinationToMainCamera(List<GameObject> i_Enemies)
    {
        NavMeshAgent navMeshAgentComponent;

        foreach (GameObject enemy in i_Enemies)
        {
            if (enemy.activeSelf && !enemy.GetComponent<Animator>().GetBool("isDead"))
            {
                navMeshAgentComponent = enemy.GetComponent<NavMeshAgent>();
                if (navMeshAgentComponent.isStopped)
                {
                    navMeshAgentComponent.ResetPath();
                }
                // update destination of enemy's NavMeshAgent
                navMeshAgentComponent.destination = GameObject.Find("Main Camera").transform.position;
            }
        }
    }

    private void setStorageOfEasyEnemiesToSpawn()
    {
        setStorageOfEnemiesToSpawn(m_EasyEnemiesToSpawn, m_EasyEnemiesToSpawnStorage, "StorageOfEasyEnemiesToSpawn");
    }

    private void setStorageOfAdvancedEnemiesToSpawn()
    {
        setStorageOfEnemiesToSpawn(m_AdvancedEnemiesToSpawn, m_AdvancedEnemiesToSpawnStorage, "StorageOfAdvancedEnemiesToSpawn");
    }
}