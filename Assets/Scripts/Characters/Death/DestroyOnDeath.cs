using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyOnDeath : MonoBehaviour
{
    private Characters.Health.IDamageable health;

    private void Awake()
    {
        health = GetComponent<Characters.Health.IDamageable>();
        if (health == null)
        {
            Debug.LogWarning($"{nameof(DestroyOnDeath)} on {name} is missing IDamageable.");
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
        Destroy(gameObject);
    }
}
