using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    ActionBasedController m_Controller;
    [SerializeField] private Hand m_Hand;

    void Start()
    {
        m_Controller = GetComponent<ActionBasedController>();
    }

    void Update()
    {
        m_Hand.SetGrip(m_Controller.selectAction.action.ReadValue<float>());
        m_Hand.SetTrigger(m_Controller.activateAction.action.ReadValue<float>());
    }
}