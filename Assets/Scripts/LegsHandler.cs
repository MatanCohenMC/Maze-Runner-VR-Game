// This script and the Legs gameobject (under the Player gameobject) are not in use.
// It's kept for future use.

using UnityEngine;

public class LegsHandler : MonoBehaviour
{
    [SerializeField] private Transform m_PlayerHead; // Reference to the player that the "legs" should follow.

    private Transform m_LegsTransform;

    private void Start()
    {
        m_LegsTransform = transform; // Cache the transform of the "legs" GameObject.
    }

    private void Update()
    {
        if (m_PlayerHead != null)
        {
            // Calculate the new position for the "legs" GameObject.
            Vector3 newPosition = new Vector3(m_PlayerHead.position.x, m_LegsTransform.position.y, m_PlayerHead.position.z);

            // Set the position of the "legs" GameObject to maintain the y-position.
            m_LegsTransform.position = newPosition;
        }
    }
}