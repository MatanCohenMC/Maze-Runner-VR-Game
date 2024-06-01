using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private HealthManager m_healthManager;

    private void OnTriggerStay(Collider other)
    {
        if (tag == "Enemy" && !GetComponent<Animator>().GetBool("isDead") && other.tag == "Wall") // if its the enemy's collider and enemy isnt dead that gets into a wall
        {
            NavMeshAgent navMeshAgentComponent = GetComponent<NavMeshAgent>();

            if (navMeshAgentComponent != null)
            {
                // reset path
                navMeshAgentComponent.ResetPath();
            }
        }
    }

    public void MakeEnemyDeactivate()
    {
        gameObject.SetActive(false);
        GameObject.Find("EnemiesSpawner").GetComponent<EnemiesSpawnerManager>().DecreaseEnemyCountByOneAndUpdateSpawnTimeNextEnemy();
    }
}