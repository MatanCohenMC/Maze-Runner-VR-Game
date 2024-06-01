using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private HealthBar m_HealthBar;
    [SerializeField] private float m_MaxHealth;

    private float m_CurrentHealth;
    public event Action OnDeath;

    private void Start()
    {
        GameManager.OnPlayModeStart += ResetHealth;
    }

    private void Awake()
    {
        m_HealthBar.SetMaxHealth(m_MaxHealth);
        m_CurrentHealth = m_MaxHealth;
    }

    public float GetHealth()
    {
        return m_CurrentHealth;
    }

    public void TakeDamage(float i_DamagePoints)
    {
        m_CurrentHealth -= i_DamagePoints;
        if (m_CurrentHealth < 0)
        {
            m_CurrentHealth = 0;
        }

        m_HealthBar.SetHealth(m_CurrentHealth);
        if (m_CurrentHealth == 0)
        {
            die();
        }
    }

    public void Heal(int i_HealthPoints)
    {
        m_CurrentHealth += i_HealthPoints;

        if (m_CurrentHealth > m_MaxHealth)
        {
            m_CurrentHealth = m_MaxHealth;
        }

        m_HealthBar.SetHealth(m_CurrentHealth);
    }

    public void ResetHealth()
    {
        m_CurrentHealth = m_MaxHealth;
        m_HealthBar.SetHealth(m_MaxHealth);
    }

    private void die()
    {
        OnDeath?.Invoke();
    }
}