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

        if (deathScreenRoot != null)
        {
            deathScreenRoot.SetActive(false);
        }
        if (hudWindowRoot != null)
        {
            hudWindowRoot.SetActive(true);
        }

        if (playerHealth == null)
        {
            Debug.LogWarning($"{nameof(DeathScreen)} on {name} is missing a valid player health source.");
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
        if (hudWindowRoot != null)
        {
            hudWindowRoot.SetActive(false);
        }

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
}
