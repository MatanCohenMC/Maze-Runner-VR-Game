using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider m_Slider;

    private float m_MaxHealth;

    public void SetMaxHealth(float i_MaxHealth)
    {
        m_MaxHealth = i_MaxHealth;
    }

    public void SetHealth(float i_Health)
    {
        m_Slider.value = i_Health/m_MaxHealth;
    }
}