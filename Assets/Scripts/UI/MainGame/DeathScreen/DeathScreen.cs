using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameObject deathScreenRoot;
    [SerializeField] private GameObject hudWindowRoot;
    private IDamageable playerHealth;

    [SerializeField] private bool pauseGameOnDeath = true;

    private bool isShown;
    private bool changedTimeScale;

    private void Awake()
    {
        playerHealth = mainCharacter != null ? mainCharacter.GetComponent<IDamageable>() : null;
        SetActiveIfAssigned(deathScreenRoot, false);
        SetActiveIfAssigned(hudWindowRoot, true);

        if (playerHealth == null)
        {
            enabled = false;
        }
    }
    
    private void OnEnable()
    {
        if (playerHealth == null) return;
        playerHealth.OnHealthChanged += OnHealthChanged;
        playerHealth.OnDeath += ShowDeathScreen;
    }

    private void Start()
    {
        if (playerHealth == null)
        {
            return;
        }

        OnHealthChanged(playerHealth.CurrentHealth);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnHealthChanged;
            playerHealth.OnDeath -= ShowDeathScreen;
        }

        RestoreTimeScaleIfChanged();
    }

    public void ShowDeathScreen()
    {
        if (isShown)
        {
            return;
        }

        isShown = true;
        SetActiveIfAssigned(deathScreenRoot, true);
        SetActiveIfAssigned(hudWindowRoot, false);

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
            changedTimeScale = true;
        }
    }

    private void OnHealthChanged(float currentHealth)
    {
        if (currentHealth <= 0f)
        {
            ShowDeathScreen();
        }
    }

    private void RestoreTimeScaleIfChanged()
    {
        if (!changedTimeScale)
        {
            return;
        }

        Time.timeScale = 1f;
        changedTimeScale = false;
    }

    private void SetActiveIfAssigned(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }
}
