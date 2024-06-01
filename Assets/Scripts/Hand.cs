using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    Animator m_Animator;
    SkinnedMeshRenderer m_Mesh;
    private float m_GripTarget;
    private float m_TriggerTarget;
    private float m_GripCurrent;
    private float m_TriggerCurrent;
    private readonly string r_AnimatorGripParam = "Grip";
    private readonly string r_AnimatorTriggerParam = "Trigger";

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        animateHand();
    }

    internal void SetGrip(float v)
    {
        m_GripTarget = v;
    }

    internal void SetTrigger(float v)
    {
        m_TriggerTarget = v;
    }

    private void animateHand()
    {
        if (m_GripCurrent != m_GripTarget)
        {
            m_GripCurrent = Mathf.MoveTowards(m_GripCurrent, m_GripTarget, Time.deltaTime * m_Speed);
            m_Animator.SetFloat(r_AnimatorGripParam, m_GripCurrent);
        }
        if (m_TriggerCurrent != m_TriggerTarget)
        {
            m_TriggerCurrent = Mathf.MoveTowards(m_TriggerCurrent, m_TriggerTarget, Time.deltaTime * m_Speed);
            m_Animator.SetFloat(r_AnimatorTriggerParam, m_TriggerCurrent);
        }
    }

    public void ToggleVisibility()
    {
        m_Mesh.enabled = !m_Mesh.enabled;
    }
}