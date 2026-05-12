using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private GameObject hudWindowRoot;
    [SerializeField] private bool pauseGameOnWin = true;

    private IDamageable playerHealth;
    private bool isShown;
    private bool changedTimeScale;
    private int initialEnemyCount;

    private void Awake()
    {
        playerHealth = mainCharacter != null ? mainCharacter.GetComponent<IDamageable>() : null;
        initialEnemyCount = CountEnemiesInScene();

        if (winScreenRoot != null)
        {
            winScreenRoot.SetActive(false);
        }

        if (playerHealth == null)
        {
            Debug.LogWarning($"{nameof(WinScreen)} on {name} is missing a valid player health source.");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.OnDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }

        if (changedTimeScale)
        {
            Time.timeScale = 1f;
            changedTimeScale = false;
        }
    }

    private void Update()
    {
        if (isShown || playerHealth == null || playerHealth.CurrentHealth <= 0f)
        {
            return;
        }

        if (AreAllEnemiesDefeated())
        {
            ShowWinScreen();
        }
    }

    public void ShowWinScreen()
    {
        if (isShown)
        {
            return;
        }

        isShown = true;

        if (winScreenRoot != null)
        {
            winScreenRoot.SetActive(true);
        }

        if (hudWindowRoot != null)
        {
            hudWindowRoot.SetActive(false);
        }

        if (pauseGameOnWin)
        {
            Time.timeScale = 0f;
            changedTimeScale = true;
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        if (initialEnemyCount <= 0)
        {
            return false;
        }

        return CountAliveEnemiesInScene() == 0;
    }

    private int CountEnemiesInScene()
    {
        Health[] allHealth = FindObjectsOfType<Health>();
        int enemyCount = 0;

        for (int i = 0; i < allHealth.Length; i++)
        {
            if (allHealth[i].Team == Team.Enemy)
            {
                enemyCount++;
            }
        }

        return enemyCount;
    }

    private int CountAliveEnemiesInScene()
    {
        Health[] allHealth = FindObjectsOfType<Health>();
        int aliveEnemyCount = 0;

        for (int i = 0; i < allHealth.Length; i++)
        {
            Health health = allHealth[i];
            if (health.Team == Team.Enemy && health.CurrentHealth > 0f)
            {
                aliveEnemyCount++;
            }
        }

        return aliveEnemyCount;
    }
    
    private void HandlePlayerDeath()
    {
        enabled = false;
    }
}
