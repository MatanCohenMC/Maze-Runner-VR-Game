using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolSoundController : MonoBehaviour
{
    [SerializeField] private Transform m_TipTransform; // Reference to the tip transform.
    [SerializeField] private AudioClip m_AudioClip;
    [SerializeField] private bool m_UseVelocity = true;
    [SerializeField] private float m_VelocityThreshold = 15f; // Adjust this value to control the velocity threshold.
    [SerializeField] private float m_DistanceThreshold = 1f; // Adjust this value to control the distance threshold.
    [SerializeField] private float m_MinVelocity = 15;
    [SerializeField] private float m_MaxVelocity = 30;
    [SerializeField] private float m_MaxVolume = 1.0f; // Maximum volume for the sound.

    private AudioSource m_AudioSource;
    private XRGrabInteractable m_GrabInteractable; // Reference to the XRGrabInteractable component.
    private Vector3 m_PreviousTipPosition; // Store the previous tip position.
    private float m_DistanceTraveled; // Cumulative distance traveled during a swing.
    private bool m_IsSwinging; // Track if a swing is in progress.

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_GrabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void Update()
    {
        // Check if the tool is currently held by the player.
        if (isHeldByPlayer())
        {
            Vector3 tipVelocity = (m_TipTransform.position - m_PreviousTipPosition) / Time.deltaTime;

            if (m_UseVelocity)
            {
                float tipVelocityMagnitude = tipVelocity.magnitude;
                float volume = Mathf.InverseLerp(m_MinVelocity, m_MaxVelocity, tipVelocityMagnitude) * m_MaxVolume;
                //Debug.Log($"tipVelocityMagnitude: {tipVelocityMagnitude}");
                //Debug.Log($"volume: {volume}");

                // Check if the tool's velocity exceeds the velocity threshold.
                if (tipVelocityMagnitude > m_VelocityThreshold)
                {
                    if (!m_IsSwinging)
                    {
                        m_IsSwinging = true;
                        m_DistanceTraveled = 0f; // Reset distance traveled at the start of the swing.
                    }

                    // Calculate the distance traveled during the current swing.
                    m_DistanceTraveled += Vector3.Distance(m_TipTransform.position, m_PreviousTipPosition);

                    if (!m_AudioSource.isPlaying && m_DistanceTraveled >= m_DistanceThreshold)
                    {
                        m_AudioSource.PlayOneShot(m_AudioClip, volume);
                    }
                }
                else
                {
                    m_IsSwinging = false; // Reset swing state when the tip's velocity drops.
                }
            }
            else
            {
                m_AudioSource.PlayOneShot(m_AudioClip);
            }

            // Store the current tip position for the next frame.
            m_PreviousTipPosition = m_TipTransform.position;
        }
        else
        {
            m_IsSwinging = false; // Reset swing state if not held by the player.
        }
    }

    private bool isHeldByPlayer()
    {
        // Implement logic to check if the tool is held by the player using the XRGrabInteractable component.
        return (m_GrabInteractable != null && m_GrabInteractable.isSelected);
    }
}