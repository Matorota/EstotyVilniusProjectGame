using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    public float MaxHealth => maxHealth;
    
    public float CurrentHealth => currentHealth;

    public event Action<float> OnHealthChanged;
    public void TakeDamage(float amount)
    {
        float damage = Mathf.Max(0f, amount);

        if (damage <= 0f || currentHealth <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
}
