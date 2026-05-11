using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyOnDeath : MonoBehaviour
{
    private Characters.Health.IDamageable health;

    private void Awake()
    {
        health = GetComponent<Characters.Health.IDamageable>();
    }

    private void OnEnable()
    {

        health.OnDeath += HandleDeath;
    }
    
    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
