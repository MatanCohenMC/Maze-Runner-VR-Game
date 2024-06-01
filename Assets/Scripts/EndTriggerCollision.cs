using UnityEngine;

public class EndTriggerCollision : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("End Trigger was activated");
        GameManager.Instance.EndGame(eGameOver.Win); // Player won
    }
}
