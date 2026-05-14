using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private GameObject hudWindowRoot;
    [SerializeField] private bool pauseGameOnWin = true;

    private IDamageable playerHealth;
    private bool isShown;
    public bool HasWon;
    private bool changedTimeScale;
    private int initialEnemyCount;
    private Health[] enemyHealthSources;
    private int aliveEnemyCount;
    private bool enemiesInitialized;

    private void Awake()
    {
        playerHealth = mainCharacter != null ? mainCharacter.GetComponent<IDamageable>() : null;
        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(hudWindowRoot, true);

        if (playerHealth == null)
        {
            Debug.LogWarning($"{nameof(WinScreen)} on {name} is missing a valid player health source.");
            enabled = false;
        }
    }

    private void Start()
    {
        InitializeEnemies();
        TryShowWinScreen();
    }

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.OnDeath += HandlePlayerDeath;
        if (enemiesInitialized)
        {
            SubscribeToEnemyDeaths();
            aliveEnemyCount = CountAliveEnemies(enemyHealthSources);
            TryShowWinScreen();
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
        if (enemiesInitialized)
        {
            UnsubscribeFromEnemyDeaths();
        }

        RestoreTimeScaleIfChanged();
    }

    public void ShowWinScreen()
    {
        if (isShown)
        {
            return;
        }

        isShown = true;
        HasWon = true;
        SetActiveIfAssigned(winScreenRoot, true);
        SetActiveIfAssigned(hudWindowRoot, false);

        if (pauseGameOnWin)
        {
            Time.timeScale = 0f;
            changedTimeScale = true;
        }
    }

    private void TryShowWinScreen()
    {
        if (isShown || playerHealth == null || playerHealth.CurrentHealth <= 0f)
        {
            return;
        }

        if (initialEnemyCount <= 0 || aliveEnemyCount > 0)
        {
            return;
        }

        ShowWinScreen();
    }

    private void SubscribeToEnemyDeaths()
    {
        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            Health enemyHealth = enemyHealthSources[i];
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void InitializeEnemies()
    {
        enemyHealthSources = FindEnemyHealthSources();
        initialEnemyCount = enemyHealthSources.Length;
        aliveEnemyCount = CountAliveEnemies(enemyHealthSources);
        enemiesInitialized = true;

        if (isActiveAndEnabled)
        {
            SubscribeToEnemyDeaths();
        }
    }

    private void UnsubscribeFromEnemyDeaths()
    {
        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            Health enemyHealth = enemyHealthSources[i];
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath -= HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath()
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        TryShowWinScreen();
    }

    private Health[] FindEnemyHealthSources()
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

        Health[] enemyHealth = new Health[enemyCount];
        int enemyIndex = 0;
        for (int i = 0; i < allHealth.Length; i++)
        {
            if (allHealth[i].Team == Team.Enemy)
            {
                enemyHealth[enemyIndex] = allHealth[i];
                enemyIndex++;
            }
        }

        return enemyHealth;
    }

    private int CountAliveEnemies(Health[] enemyHealthSources)
    {
        int aliveCount = 0;

        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            Health enemyHealth = enemyHealthSources[i];
            if (enemyHealth != null && enemyHealth.CurrentHealth > 0f)
            {
                aliveCount++;
            }
        }

        return aliveCount;
    }
    
    private void HandlePlayerDeath()
    {
        enabled = false;
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
