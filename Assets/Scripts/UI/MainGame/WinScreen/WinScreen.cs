using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private GameObject hudWindowRoot;
    [SerializeField] private bool pauseGameOnWin = true;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private EndQuestButtonManager endQuestButtonManager;

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

    private void Update()
    {
        // Refresh enemy list for dynamically spawned enemies
        RefreshEnemyList();
    }

    private void RefreshEnemyList()
    {
        Health[] currentEnemies = FindEnemyHealthSources();
        
        if (currentEnemies.Length != enemyHealthSources.Length)
        {
            Debug.Log($"WinScreen: Enemy count changed from {enemyHealthSources.Length} to {currentEnemies.Length}");
            // Enemy count changed - update the list
            UnsubscribeFromEnemyDeaths();
            enemyHealthSources = currentEnemies;
            initialEnemyCount = enemyHealthSources.Length;
            aliveEnemyCount = CountAliveEnemies(enemyHealthSources);
            SubscribeToEnemyDeaths();
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
        Debug.Log($"ShowWinScreen: Showing win screen root, hudWindowRoot active: {(hudWindowRoot != null && hudWindowRoot.activeInHierarchy)}");
        SetActiveIfAssigned(winScreenRoot, true);
        SetActiveIfAssigned(hudWindowRoot, false);

        // Make sure all UI elements in the win screen are visible
        if (winScreenRoot != null)
        {
            CanvasGroup canvasGroup = winScreenRoot.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                Debug.Log("ShowWinScreen: CanvasGroup configured");
            }

            // Make all child buttons visible
            Button[] buttons = winScreenRoot.GetComponentsInChildren<Button>(true);
            Debug.Log($"ShowWinScreen: Found {buttons.Length} buttons in win screen");
            foreach (Button btn in buttons)
            {
                btn.gameObject.SetActive(true);
                Debug.Log($"ShowWinScreen: Enabled button: {btn.name}");
            }
        }

        if (pauseGameOnWin)
        {
            Time.timeScale = 0f;
            changedTimeScale = true;
        }
        
        Debug.Log("ShowWinScreen: Win screen fully configured!");
    }

    private void TryShowWinScreen()
    {
        if (isShown || playerHealth == null || playerHealth.CurrentHealth <= 0f)
        {
            return;
        }

        Debug.Log($"WinScreen: Checking win condition - initialEnemyCount: {initialEnemyCount}, aliveEnemyCount: {aliveEnemyCount}");

        if (initialEnemyCount <= 0 || aliveEnemyCount > 0)
        {
            return;
        }

        Debug.Log("WinScreen: All enemies dead! Showing win screen!");
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
        Debug.Log($"WinScreen: Enemy died! Alive: {aliveEnemyCount}/{initialEnemyCount}");
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

    public void OnContinueButtonPressed()
    {
        Debug.Log("WinScreen: Continue button pressed! Resuming game and showing End Quest button...");
        
        // Call PauseMenu.Resume() to unpause and show HUD
        if (pauseMenu != null)
        {
            pauseMenu.Resume();
            Debug.Log("WinScreen: PauseMenu.Resume() called");
        }
        else
        {
            Debug.LogWarning("WinScreen: PauseMenu reference not assigned in inspector!");
        }
        
        // Show the End Quest button via EndQuestButtonManager
        if (endQuestButtonManager != null)
        {
            endQuestButtonManager.ShowEndQuestButton();
            Debug.Log("WinScreen: Called EndQuestButtonManager.ShowEndQuestButton()");
        }
        else
        {
            Debug.LogWarning("WinScreen: EndQuestButtonManager reference not assigned in inspector!");
        }
    }
}
