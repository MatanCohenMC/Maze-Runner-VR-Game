using UnityEngine;

public class HealHandler : MonoBehaviour
{
    [SerializeField] private int m_HealPoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag is "Player")
        {
            other.GetComponent<HealthManager>().Heal(m_HealPoints);
            Debug.Log($"{other.name} got healed ({m_HealPoints} health)");
            Destroy(gameObject);
        }
    }
}