using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject m_Player;
    [SerializeField] Camera m_PlayerHead;

    [ContextMenu("Reset Position")]
    public void ResetPosition(Transform i_ResetTransform)
    {
        var rotationAngleY = i_ResetTransform.rotation.eulerAngles.y - m_PlayerHead.transform.rotation.eulerAngles.y;
        m_Player.transform.Rotate(0, rotationAngleY, 0);

        var distanceDiff = i_ResetTransform.position - m_PlayerHead.transform.position;
        m_Player.transform.position += distanceDiff;
    }
}