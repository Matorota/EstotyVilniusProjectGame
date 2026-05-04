using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameObject deathScreenRoot;
    private Health playerHealth;

    [Header("Behaviour")]
    [SerializeField] private bool pauseGameOnDeath = true;

    private bool isShown;
    private bool changedTimeScale;

    private void Awake()
    {
        playerHealth = mainCharacter != null ? mainCharacter.GetComponent<Health>() : null;

        if (deathScreenRoot != null)
        {
            deathScreenRoot.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged += OnHealthChanged;
            OnHealthChanged(playerHealth.CurrentHealth, playerHealth.HealthCapacity);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= OnHealthChanged;
        }

        if (changedTimeScale)
        {
            Time.timeScale = 1f;
            changedTimeScale = false;
        }
    }

    public void ShowDeathScreen()
    {
        if (isShown)
        {
            return;
        }

        isShown = true;

        if (deathScreenRoot != null)
        {
            deathScreenRoot.SetActive(true);
        }

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
            changedTimeScale = true;
        }
        
    }

    private void OnHealthChanged(float currentHealth, float _)
    {
        if (currentHealth <= 0f)
        {
            ShowDeathScreen();
        }
    }
}
