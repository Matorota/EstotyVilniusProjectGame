using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float healthCapacity = 100f;
    [SerializeField] float currentHealth = 100f;

    [Header("Death")]
    [SerializeField] bool destroyWhenDead;

    public float HealthCapacity => healthCapacity;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action<float, float> HealthChanged;
    public event Action Died;
    bool hasDied;

    void Awake()
    {
        ClampValues();
        NotifyHealthChanged();

        if (IsDead)
        {
            Die();
        }
    }

    void OnValidate()
    {
        ClampValues();
        NotifyHealthChanged();
    }

    public void TakeDamage(float amount)
    {
        float damage = Mathf.Max(0f, amount);

        if (damage <= 0f || hasDied)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, healthCapacity);
        NotifyHealthChanged();

        if (IsDead)
        {
            Die();
        }
    }

    void Die()
    {
        if (hasDied)
        {
            return;
        }

        hasDied = true;
        Died?.Invoke();

        if (destroyWhenDead)
        {
            Destroy(gameObject);
        }
    }

    void ClampValues()
    {
        healthCapacity = Mathf.Max(1f, healthCapacity);
        currentHealth = Mathf.Clamp(currentHealth, 0f, healthCapacity);
    }

    void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(currentHealth, healthCapacity);
    }
}
