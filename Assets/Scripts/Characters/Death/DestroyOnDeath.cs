using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyOnDeath : MonoBehaviour
{
    private Health health;
    private bool hasDestroyed;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += OnHealthChanged;
        TryDestroyIfDepleted(health.CurrentHealth);
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth)
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
