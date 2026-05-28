using System;
using System.Collections.Generic;
using Configs;
using UI.Windows;
using UnityEngine;

public class QuestRunner : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private GameHubWindow hubWindow;
    [SerializeField] private WinQuestWindow winQuestWindow;
    [SerializeField] private EndQuestWidget endQuestWidget;
    

    private readonly HashSet<Health> trackedEnemies = new HashSet<Health>();
    private readonly Dictionary<Health, Action> enemyDeathHandlers = new Dictionary<Health, Action>();

    private int spawnedEnemies;
    private Health playerHealth;
    private PlayerLifecycle playerLifecycle;
    
    public QuestConfig CurrentQuest { get; private set; }
    public QuestStatus Status { get; private set; }
    public int AliveEnemies { get; private set; }
    public int TotalEnemies { get; private set; }

    public event Action<QuestConfig> OnQuestStarted;
    public event Action OnQuestWon;
    public event Action OnQuestEnded;
    public event Action OnAliveCountChanged;

    private void Awake()
    {
        if (playerLifecycle == null)
            playerLifecycle = PlayerLifecycle.Instance;
        
        PauseGame();
        playerLifecycle?.DisableMovement();

        HideEndQuestButton();
    }

    private void OnEnable()
    {
        if (playerLifecycle == null)
            playerLifecycle = PlayerLifecycle.Instance;

        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied -= HandlePlayerDied;

        UnsubscribeFromEnemySpawner();
        ClearTrackedEnemies();
        CurrentQuest = null;
        Status = QuestStatus.None;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;
    }

    private void AbortQuestStart()
    {
        UnsubscribeFromEnemySpawner();
        ClearTrackedEnemies();
        HideEndQuestButton();
        CurrentQuest = null;
        Status = QuestStatus.None;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;
        OnAliveCountChanged?.Invoke();

        PauseGame();
        playerLifecycle?.DisableMovement();
    }

    public void StartQuest(QuestConfig quest)
    {
        CurrentQuest = quest;
        Status = QuestStatus.Active;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = Mathf.Max(0, quest.EnemiesAmount);

        ClearTrackedEnemies();
        SubscribeToEnemySpawner();

        if (TotalEnemies <= 0)
        {
            CompleteQuest();
            return;
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner == null)
            {
                AbortQuestStart();
                return;
            }
        }

        bool started = enemySpawner.SpawnEnemies(quest.EnemiesAmount);
        if (!started)
        {
            AbortQuestStart();
            return;
        }

        ResumeGame();
        ShowHud();
        HideEndQuestButton();
        EnablePlayerMovement();

        OnQuestStarted?.Invoke(quest);
        OnAliveCountChanged?.Invoke();
    }

    public void EndQuest()
    {
        HealPlayer();
        DestroyAllCardPickups();
        ClearTrackedEnemies();
        UnsubscribeFromEnemySpawner();

        OnQuestEnded?.Invoke();

        CurrentQuest = null;
        Status = QuestStatus.None;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;
        OnAliveCountChanged?.Invoke();

        PauseGame();
        playerLifecycle?.DisableMovement();
    }

    private void DestroyAllCardPickups()
    {
        CardPickup[] allPickups = FindObjectsByType<CardPickup>(FindObjectsSortMode.None);
        GameObject playerObj = playerLifecycle != null ? playerLifecycle.gameObject : null;

        foreach (CardPickup pickup in allPickups)
        {
            if (pickup != null && pickup.gameObject != playerObj)
            {
                Destroy(pickup.gameObject);
            }
        }
    }

    private void HandleEnemySpawned(Health health)
    {
        if (Status != QuestStatus.Active || health == null || health.Team != Team.Enemy)
        {
            return;
        }

        if (trackedEnemies.Contains(health))
        {
            return;
        }

        trackedEnemies.Add(health);
        spawnedEnemies++;
        AliveEnemies++;

        Action deathHandler = () => HandleEnemyDeath(health);
        enemyDeathHandlers[health] = deathHandler;
        health.OnDeath += deathHandler;

        OnAliveCountChanged?.Invoke();
        EvaluateQuestCompletion();
    }

    private void HandleEnemyDeath(Health health)
    {
        if (Status != QuestStatus.Active || health == null)
        {
            return;
        }

        if (!trackedEnemies.Remove(health))
        {
            return;
        }

        if (enemyDeathHandlers.TryGetValue(health, out Action deathHandler))
        {
            health.OnDeath -= deathHandler;
            enemyDeathHandlers.Remove(health);
        }

        AliveEnemies = Mathf.Max(0, AliveEnemies - 1);
        OnAliveCountChanged?.Invoke();
        EvaluateQuestCompletion();
    }

    private void EvaluateQuestCompletion()
    {
        if (Status != QuestStatus.Active)
        {
            return;
        }

        if (TotalEnemies > 0 && spawnedEnemies >= TotalEnemies && AliveEnemies <= 0)
        {
            CompleteQuest();
        }
    }

    private void HandlePlayerDied()
    {
        if (Status != QuestStatus.Active)
        {
            return;
        }

        AbortQuest();
    }

    private void CompleteQuest()
    {
        if (Status != QuestStatus.Active)
        {
            return;
        }

        Status = QuestStatus.Completed;
        HideEndQuestButton();
        OnQuestWon?.Invoke();
        
        ShowWinQuestWindow();
        PauseGame();
    }

    private void AbortQuest()
    {
        if (Status != QuestStatus.Active)
        {
            return;
        }

        UnsubscribeFromEnemySpawner();
        ClearTrackedEnemies();
        HideEndQuestButton();

        CurrentQuest = null;
        Status = QuestStatus.None;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;

        OnAliveCountChanged?.Invoke();
        OnQuestEnded?.Invoke();

        PauseGame();
        playerLifecycle?.DisableMovement();
    }

    private void ShowWinQuestWindow()
    {
        if (winQuestWindow == null)
            winQuestWindow = ResolveWinQuestWindow();

        winQuestWindow?.ShowWindow();
    }

    private WinQuestWindow ResolveWinQuestWindow()
    {
        foreach (WinQuestWindow window in Resources.FindObjectsOfTypeAll<WinQuestWindow>())
        {
            if (window != null && window.gameObject.scene.IsValid())
                return window;
        }

        return null;
    }

    private void SubscribeToEnemySpawner()
    {
        EnemySpawner.OnEnemySpawned += HandleEnemySpawned;
    }

    private void UnsubscribeFromEnemySpawner()
    {
        EnemySpawner.OnEnemySpawned -= HandleEnemySpawned;
    }

    private void ClearTrackedEnemies()
    {
        foreach (KeyValuePair<Health, Action> entry in enemyDeathHandlers)
        {
            if (entry.Key != null)
                entry.Key.OnDeath -= entry.Value;
        }

        enemyDeathHandlers.Clear();
        trackedEnemies.Clear();
    }

    private void HealPlayer()
    {
        if (playerHealth == null)
        {
            Health[] all = FindObjectsOfType<Health>();
            foreach (Health h in all)
            {
                if (h != null && h.Team == Team.Player)
                {
                    playerHealth = h;
                    break;
                }
            }
        }

        if (playerHealth == null)
            return;

        float missing = playerHealth.MaxHealth - playerHealth.CurrentHealth;
        if (missing > 0f)
            playerHealth.Heal(missing);
    }

    private void ResumeGame()
    {
        if (timeScaleManager != null)
            timeScaleManager.Resume();
        else
            Time.timeScale = 1f;
    }

    public void ShowHud()
    {
        if (hubWindow == null)
            hubWindow = FindObjectOfType<GameHubWindow>();

        hubWindow?.Open();
    }

    public void ShowEndQuestButton()
    {
        if (endQuestWidget == null)
            endQuestWidget = FindObjectOfType<EndQuestWidget>();

        endQuestWidget?.ShowButton();
    }

    private void HideEndQuestButton()
    {
        if (endQuestWidget == null)
        {
            endQuestWidget = FindObjectOfType<EndQuestWidget>();
        }
        endQuestWidget?.HideButton();
    }

    private void PauseGame()
    {
        if (timeScaleManager != null)
            timeScaleManager.Pause();
        else
            Time.timeScale = 0f;
    }

    private void EnablePlayerMovement()
    {
        playerLifecycle?.EnableMovement();
    }
}
