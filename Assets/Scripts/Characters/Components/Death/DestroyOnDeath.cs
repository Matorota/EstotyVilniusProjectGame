using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyOnDeath : MonoBehaviour
{
    Health health;
    bool hasDestroyed;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
        TryDestroyIfDepleted(health.CurrentHealth);
    }

    void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
    }

    void OnHealthChanged(float currentHealth, float _)
    {
        TryDestroyIfDepleted(currentHealth);
    }

    void TryDestroyIfDepleted(float currentHealth)
    {
        if (hasDestroyed || currentHealth > 0f)
        {
            return;
        }

        hasDestroyed = true;
        Destroy(gameObject);
    }
}
