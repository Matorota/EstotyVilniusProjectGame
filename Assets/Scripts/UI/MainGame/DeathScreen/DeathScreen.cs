using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameObject deathScreenRoot;
    private Health playerHealth;

    [SerializeField] private bool pauseGameOnDeath = true;

    private bool isShown;
    private bool changedTimeScale;

    private void Awake()
    {
        playerHealth = mainCharacter.GetComponent<Health>();
        deathScreenRoot.SetActive(false);
    }
    
    private void OnEnable()
    {
        playerHealth.OnHealthChanged += OnHealthChanged;
        OnHealthChanged(playerHealth.CurrentHealth);
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= OnHealthChanged;

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

        deathScreenRoot.SetActive(true);

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
