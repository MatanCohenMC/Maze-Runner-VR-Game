// This script and the Legs gameobject (under the Player gameobject) are not in use.
// It's kept for future use.

using UnityEngine;

public class FootstepsHandler : MonoBehaviour
{
    [SerializeField] private AudioSource m_FootstepAudioSource;
    [SerializeField] private AudioClip[] m_FootstepClips;
    [SerializeField] private float m_StepInterval = 0.5f;
    [SerializeField] private float m_MinMoveMagnitude = 0.03f; // Minimum movement magnitude to trigger footstep sounds.

    private Transform m_LegsTransform; // Reference to the "Legs" GameObject's transform.
    private Vector3 m_PreviousPosition; // Store the previous position for magnitude calculation.
    private float m_StepCooldown = 0f;

    private void Start()
    {
        m_LegsTransform = transform; // Cache the transform of the "Legs" GameObject.
        m_PreviousPosition = m_LegsTransform.position;
    }

    private void FixedUpdate()
    {
        // Calculate the movement vector based on the position difference between frames.
        Vector3 movement = m_LegsTransform.position - m_PreviousPosition;
        // Calculate the magnitude of movement (how far the "Legs" GameObject moved in one frame).
        float movementMagnitude = movement.magnitude;

        if (movementMagnitude > m_MinMoveMagnitude && Time.time > m_StepCooldown)
        {
            playFootstepSound();
            m_StepCooldown = Time.time + m_StepInterval;
        }

        // Update the previous position for the next frame.
        m_PreviousPosition = m_LegsTransform.position;
    }

    private void playFootstepSound()
    {
        if (m_FootstepClips.Length == 0 || m_FootstepAudioSource == null)
        {
            return;
        }

        AudioClip randomClip = m_FootstepClips[Random.Range(0, m_FootstepClips.Length)];
        m_FootstepAudioSource.PlayOneShot(randomClip);
    }
}