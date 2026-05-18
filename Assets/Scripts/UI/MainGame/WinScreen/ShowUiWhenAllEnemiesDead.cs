using UnityEngine;

public class ShowUiWhenAllEnemiesDead : MonoBehaviour
{

    [SerializeField] private GameObject uiRoot;
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private bool pauseGameOnShow = true;

    private Health[] enemyHealthSources;
    private int aliveEnemyCount;
    private int initialEnemyCount;
    private bool changedTimeScale;
    public void CloseUi()
    {
        HideUi();
        enabled = false; 
    }
    private void Awake()
    {
        SetActiveIfAssigned(uiRoot, false);
    }

    private void Start()
    {
        InitializeEnemies();
        RefreshUiState();
    }

    private void OnEnable()
    {
        if (enemyHealthSources != null)
        {
            SubscribeToEnemyDeaths();
        }

        RefreshUiState();
    }

    private void OnDisable()
    {
        UnsubscribeFromEnemyDeaths();
        HideUi();
    }

    private void Update()
    {
        RefreshUiState();
    }

    private void InitializeEnemies()
    {
        enemyHealthSources = FindEnemyHealthSources();
        initialEnemyCount = enemyHealthSources.Length;
        aliveEnemyCount = CountAliveEnemies(enemyHealthSources);

        if (isActiveAndEnabled)
        {
            SubscribeToEnemyDeaths();
        }
    }

    private void SubscribeToEnemyDeaths()
    {
        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            Health enemy = enemyHealthSources[i];
            if (enemy != null)
            {
                enemy.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void UnsubscribeFromEnemyDeaths()
    {
        if (enemyHealthSources == null)
        {
            return;
        }

        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            Health enemy = enemyHealthSources[i];
            if (enemy != null)
            {
                enemy.OnDeath -= HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath()
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        RefreshUiState();
    }

    private void RefreshUiState()
    {
        bool allEnemiesDead = initialEnemyCount > 0 && aliveEnemyCount <= 0;
        bool isHudVisible = hudRoot != null && hudRoot.activeInHierarchy;
        bool shouldShow = allEnemiesDead && !isHudVisible;

        if (shouldShow)
        {
            ShowUi();
        }
        else
        {
            HideUi();
        }
    }

    private void ShowUi()
    {
        SetActiveIfAssigned(uiRoot, true);
        if (pauseGameOnShow && !changedTimeScale)
        {
            Time.timeScale = 0f;
            changedTimeScale = true;
        }
    }

    private void HideUi()
    {
        SetActiveIfAssigned(uiRoot, false);
        if (changedTimeScale)
        {
            RestoreTimeScaleIfChanged();
        }
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

        Health[] enemies = new Health[enemyCount];
        int index = 0;

        for (int i = 0; i < allHealth.Length; i++)
        {
            if (allHealth[i].Team == Team.Enemy)
            {
                enemies[index] = allHealth[i];
                index++;
            }
        }

        return enemies;
    }

    private int CountAliveEnemies(Health[] enemies)
    {
        int alive = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            Health enemy = enemies[i];
            if (enemy != null && enemy.CurrentHealth > 0f)
            {
                alive++;
            }
        }

        return alive;
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
