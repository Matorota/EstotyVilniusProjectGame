using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyOnDeath : MonoBehaviour
{
    Health health;
    bool hasDestroyed;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
        TryDestroyIfDepleted(health.CurrentHealth);
    }

    private void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth, float _)
    {
        TryDestroyIfDepleted(currentHealth);
    }

    private void TryDestroyIfDepleted(float currentHealth)
    {
        if (hasDestroyed || currentHealth > 0f)
        {
            return;
        }

        hasDestroyed = true;
        Destroy(gameObject);
    }
}
