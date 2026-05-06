using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyOnDeath : MonoBehaviour
{
    private Health health;
    private bool hasDestroyed;

    private void OnEnable()
    {
        health ??= GetComponent<Health>();
        health.OnHealthChanged += OnHealthChanged;
        OnHealthChanged(health.CurrentHealth);
    }
    
    private void OnDisable()
    {
        health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float _)
    {
        if (!health.CanBeDestroyed(hasDestroyed))
        {
            return;
        }

        hasDestroyed = true;
        Destroy(gameObject);
    }
}
