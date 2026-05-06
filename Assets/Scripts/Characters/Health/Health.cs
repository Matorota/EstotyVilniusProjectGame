using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public event Action<float> OnHealthChanged;

    public bool CanBeDestroyed(bool hasDestroyed)
    {
        return !hasDestroyed && currentHealth <= 0f;
    }

    public void TakeDamage(float amount)
    {
        float damage = amount;
        if (damage < 0f)
        {
            damage = 0f;
        }

        if (damage <= 0f || currentHealth <= 0f)
        {
            return;
        }

        float updatedHealth = currentHealth - damage;
        if (updatedHealth < 0f)
        {
            updatedHealth = 0f;
        }
        else if (updatedHealth > maxHealth)
        {
            updatedHealth = maxHealth;
        }

        currentHealth = updatedHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }
}
