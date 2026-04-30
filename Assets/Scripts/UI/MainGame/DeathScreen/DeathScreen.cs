using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject deathScreenRoot;

    [Header("Behaviour")]
    [SerializeField] private bool pauseGameOnDeath = true;

    private bool isShown;
    private bool changedTimeScale;

    private void OnValidate()
    {
        CacheReferences();
    }

    private void Awake()
    {
        CacheReferences();

        if (deathScreenRoot != null)
        {
            deathScreenRoot.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.Died += ShowDeathScreen;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.Died -= ShowDeathScreen;
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

    private void CacheReferences()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<Health>();
        }
    }
}
