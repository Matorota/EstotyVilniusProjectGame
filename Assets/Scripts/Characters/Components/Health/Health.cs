using System;
using UnityEngine;

public partial class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float healthCapacity = 100f;
    [SerializeField] float currentHealth = 100f;

    public float HealthCapacity => healthCapacity;
    public float CurrentHealth => currentHealth;

    public event Action<float, float> HealthChanged;

    private void Awake()
    {
        ClampValues();
        NotifyHealthChanged();
    }
    

    private void OnValidate()
    {
        ClampValues();
        NotifyHealthChanged();
    }

    public void TakeDamage(float amount)
    {
        float damage = Mathf.Max(0f, amount);

        if (damage <= 0f || currentHealth <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, healthCapacity);
        NotifyHealthChanged();
    }

    private void ClampValues()
    {
        healthCapacity = Mathf.Max(1f, healthCapacity);
        currentHealth = Mathf.Clamp(currentHealth, 0f, healthCapacity);
    }

    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(currentHealth, healthCapacity);
    }
}
