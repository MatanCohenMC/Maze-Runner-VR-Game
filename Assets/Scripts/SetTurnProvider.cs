using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SetTurnProvider : MonoBehaviour
{
    [SerializeField] private ActionBasedContinuousTurnProvider m_ContinuousTurn;
    [SerializeField] private ActionBasedSnapTurnProvider m_SnapTurn;

    public void SetProviderFromIndex(int i_Index)
    {
        if(i_Index == 0)
        {
            m_SnapTurn.enabled = false;
            m_ContinuousTurn.enabled = true;
            Debug.Log("Turn provider set to Continuous Turn");
        }
        else if(i_Index == 1)
        {
            m_SnapTurn.enabled = true;
            m_ContinuousTurn.enabled = false;
            Debug.Log("Turn provider set to Snap Turn");

        }
    }
}
