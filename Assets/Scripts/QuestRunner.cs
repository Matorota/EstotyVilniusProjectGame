using System;
using System.Collections.Generic;
using Configs;
using UI.Windows;
using UnityEngine;

public class QuestRunner : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private GameHubWindow hubWindow;
    
    private readonly HashSet<Health> trackedEnemies = new HashSet<Health>();
    private readonly  Dictionary<Health, Action> enemyDeathHandlers = new Dictionary<Health, Action>();

    private int spawnedEnemies;

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
        ResolveDependencies();
    }

    private void OnDisable()
    {
        UnsubscribeFromEnemySpawner();
        ClearTrackedEnemies();
        CurrentQuest = null;
        Status = QuestStatus.None;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;
    }

    public void StartQuest(QuestConfig quest)
    {
        ResolveDependencies();

        CurrentQuest = quest;
        Status = QuestStatus.Active;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = Mathf.Max(0, quest.EnemiesAmount);

        ClearTrackedEnemies();
        SubscribeToEnemySpawner();

        ResumeGame();
        ShowHud();
        HideEndQuestButton();

        OnQuestStarted?.Invoke(quest);
        OnAliveCountChanged?.Invoke();

        if (TotalEnemies <= 0)
        {
            Status = QuestStatus.Completed;
            OnQuestWon?.Invoke();
            return;
        }

        enemySpawner.SpawnEnemies(quest.EnemiesAmount);
    }

    public void EndQuest()
    {
        ResumeGame();
        HealPlayer();
        DestroyCardPickups();
        ClearTrackedEnemies();
        UnsubscribeFromEnemySpawner();

        ShowHud();
        OnQuestEnded?.Invoke();

        CurrentQuest = null;
        Status = QuestStatus.None;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;
        OnAliveCountChanged?.Invoke();
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
            Status = QuestStatus.Completed;
            OnQuestWon?.Invoke();
        }
    }

    private void SubscribeToEnemySpawner()
    {
        EnemySpawner.OnEnemySpawned -= HandleEnemySpawned;
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
            {
                entry.Key.OnDeath -= entry.Value;
            }
        }

        enemyDeathHandlers.Clear();
        trackedEnemies.Clear();
    }

    private void HealPlayer()
    {
        if (gameUIController == null)
        {
            return;
        }

        Health healthComp = gameUIController.GetPlayerHealth();
        if (healthComp == null)
        {
            return;
        }

        float missing = healthComp.MaxHealth - healthComp.CurrentHealth;
        if (missing > 0f)
        {
            healthComp.Heal(missing);
        }
    }

    private void DestroyCardPickups()
    {
        CharacterMovements mainCharacter = gameUIController != null ? gameUIController.GetMainCharacter() : null;
        List<CardPickup> pickups = CardPickup.GetActivePickups();

        for (int i = 0; i < pickups.Count; i++)
        {
            CardPickup pickup = pickups[i];
            if (pickup != null && pickup.gameObject != mainCharacter?.gameObject)
            {
                Destroy(pickup.gameObject);
            }
        }
    }

    private void ResumeGame()
    {
        if (timeScaleManager != null)
        {
            timeScaleManager.Resume();
            return;
        }

        Time.timeScale = 1f;
    }

    private void ShowHud()
    {
        if (hubWindow != null)
        {
            hubWindow.Open();
        }

        EndQuestWidget endQuestWidget = FindSceneComponent<EndQuestWidget>();
        if (endQuestWidget != null)
        {
            endQuestWidget.ShowButton();
        }

    }

    private void HideEndQuestButton()
    {
        EndQuestWidget endQuestWidget = FindSceneComponent<EndQuestWidget>();
        if (endQuestWidget != null)
        {
            endQuestWidget.HideButton();
        }
    }

    private void ResolveDependencies()
    { 
        enemySpawner = FindSceneComponent<EnemySpawner>();
        gameUIController = FindSceneComponent<GameUIController>();
        timeScaleManager = FindSceneComponent<TimeScale>();
        hubWindow = FindSceneComponent<GameHubWindow>();
    }

    private T FindSceneComponent<T>() where T : Component
    {
        GameObject[] roots = gameObject.scene.GetRootGameObjects(); // like findObjectByType
        for (int r = 0; r < roots.Length; r++)
        {
            T found = roots[r].GetComponentInChildren<T>(true);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}
