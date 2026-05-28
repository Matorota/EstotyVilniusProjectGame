using System;
using UnityEngine;

public class PlayerLifecycle : MonoBehaviour
{
    public static PlayerLifecycle Instance { get; private set; }

    public static void ClearInstance()
    {
        Instance = null;
    }

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

    private void Awake()
    {
        if (Instance == null || Instance.gameObject == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
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
        OnPlayerDied?.Invoke();
        DestroyAllEnemies();
    }

    public void Respawn(Transform respawnLocation)
    {
        if (respawnLocation == null)
            return;

        isDead = false;

        ResetCharacterController();
        transform.SetPositionAndRotation(respawnLocation.position, respawnLocation.rotation);
        ResetCharacterMotor();

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

    private void ResetCharacterController()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            controller.enabled = true;
        }
    }

    private void ResetCharacterMotor()
    {
        CharacterMotor motor = GetComponent<CharacterMotor>();
        motor?.ResetMotion();
    }

    private void DestroyAllEnemies()
    {
        if (QuestRunner.Instance != null)
        {
            QuestRunner.Instance.DestroyAllEnemies();
            return;
        }

        Health[] allEnemies = FindObjectsByType<Health>(FindObjectsSortMode.None);
        foreach (Health enemy in allEnemies)
        {
            if (enemy != null && enemy.Team == Team.Enemy)
                Destroy(enemy.gameObject);
        }
    }
}
