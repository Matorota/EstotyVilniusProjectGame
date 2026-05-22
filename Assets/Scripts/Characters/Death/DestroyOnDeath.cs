using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyOnDeath : MonoBehaviour
{
    private IDamageable health;
    private Health healthComponent;

    private void Awake()
    {
        health = GetComponent<IDamageable>();
        healthComponent = GetComponent<Health>();
        if (health == null)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += HandleDeath;
        }
    }
    
    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        if (healthComponent != null && healthComponent.Team == Team.Player)
        {
            return;
        }

        Destroy(gameObject);
    }
}
