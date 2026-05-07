using System;
using Characters.Health;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    private bool hasDied;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public event Action<float> OnHealthChanged;
    
    public event Action OnDeath; 
    
    private bool isDefending;

    public void SetDefending(bool value) => isDefending = value;
    
    public bool CanBeDestroyed(bool hasDestroyed)
    {
        return !hasDestroyed && currentHealth <= 0f;
    }


    public void TakeDamage(float amount)
    {
        
        if (isDefending)
        {
            return;
        }
        
        if (amount < 0f)
        {
            amount = 0f;
        }

        if (amount <= 0f || currentHealth <= 0f)
        {
            return;
        }

        float updatedHealth = currentHealth - amount;
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

        if (CanBeDestroyed(hasDied))
        {
            hasDied = true;
            OnDeath?.Invoke();
        }
    }
}
