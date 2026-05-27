using System;
using UnityEngine;

public class PlayerLifecycle : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private MonoBehaviour[] componentsToDisableOnDeath;

    private bool isDead;

    public bool IsDead => isDead;
    public event Action OnPlayerDied;
    public event Action OnPlayerRespawned;

    private void OnEnable()
    {
        ResolveHealth();
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    private void ResolveHealth()
    {
        if (health != null)
            return;

        health = GetComponent<Health>();
        if (health == null)
            health = GetComponentInChildren<Health>();
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (isDead)
            return;

        isDead = true;
        DisableComponents();
        DestroyAllEnemies();
        OnPlayerDied?.Invoke();
    }

    public void Respawn(Vector3 position)
    {
        if (!isDead)
            return;

        isDead = false;
        transform.position = position;
        
        if (health != null)
            health.Heal(health.MaxHealth);

        EnableComponents();
        OnPlayerRespawned?.Invoke();
    }

    public void DisableMovement()
    {
        CharacterMovements charMovements = GetComponent<CharacterMovements>();
        if (charMovements != null)
            charMovements.enabled = false;
    }

    public void EnableMovement()
    {
        CharacterMovements charMovements = GetComponent<CharacterMovements>();
        if (charMovements != null)
            charMovements.enabled = true;
    }

    private void DisableComponents()
    {
        if (componentsToDisableOnDeath == null || componentsToDisableOnDeath.Length == 0)
            return;

        foreach (MonoBehaviour component in componentsToDisableOnDeath)
        {
            if (component != null)
                component.enabled = false;
        }
    }

    private void EnableComponents()
    {
        if (componentsToDisableOnDeath == null || componentsToDisableOnDeath.Length == 0)
            return;

        foreach (MonoBehaviour component in componentsToDisableOnDeath)
        {
            if (component != null)
                component.enabled = true;
        }
    }

    private void DestroyAllEnemies()
    {
        Health[] allEnemies = FindObjectsByType<Health>(FindObjectsSortMode.None);
        foreach (Health enemy in allEnemies)
        {
            if (enemy != null && enemy.Team == Team.Enemy)
                Destroy(enemy.gameObject);
        }
    }
}

