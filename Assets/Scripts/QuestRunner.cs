using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;

public class QuestRunner : MonoBehaviour
{
    public static QuestRunner Instance { get; private set; }

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private PlayerLifecycle playerLifecycle;
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    private readonly HashSet<Health> trackedEnemies = new();
    private readonly Dictionary<Health, Action> enemyDeathHandlers = new();

    private int spawnedEnemies;
    private GameObject spawnedPlayer;

    public QuestConfig CurrentQuest { get; private set; }
    public QuestStatus Status { get; private set; }
    public int AliveEnemies { get; private set; }
    public int TotalEnemies { get; private set; }

    public event Action<QuestConfig> OnQuestStarted;
    public event Action OnQuestWon;
    public event Action OnQuestEnded;
    public event Action OnAliveCountChanged;
    public event Action OnPlayerDied;

    private void Awake()
    {
        Instance = this;
        PauseGame();
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
        if (quest == null)
            return;

        CurrentQuest = quest;
        Status = QuestStatus.Active;

        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = Mathf.Max(0, quest.EnemiesAmount);

        DestroyAllCardPickups();
        ClearTrackedEnemies();
        SubscribeToEnemySpawner();

        DestroyPlayer();
        SpawnPlayer();

        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied += HandlePlayerDied;

        if (enemySpawner == null)
        {
            AbortQuestStart();
            return;
        }

        if (TotalEnemies <= 0)
        {
            CompleteQuest();
            return;
        }

        bool started = enemySpawner.SpawnEnemies(quest.EnemiesAmount);

        if (!started)
        {
            AbortQuestStart();
            return;
        }

        ResumeGame();
        EnablePlayerMovement();

        OnQuestStarted?.Invoke(quest);
        OnAliveCountChanged?.Invoke();
    }

    public void EndQuest()
    {
        HealPlayer();

        DestroyAllCardPickups();
        DestroyAllEnemies();

        UnsubscribeFromEnemySpawner();
        ClearTrackedEnemies();

        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied -= HandlePlayerDied;
        DestroyPlayer();

        CurrentQuest = null;
        Status = QuestStatus.None;

        OnQuestEnded?.Invoke();

        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;

        OnAliveCountChanged?.Invoke();

        PauseGame();
    }

    public void ResetQuestState()
    {
        CurrentQuest = null;
        Status = QuestStatus.None;
        ResumeGame();
    }

    private void HandleEnemySpawned(Health health)
    {
        if (Status != QuestStatus.Active)
            return;

        if (health == null)
            return;

        if (health.Team != Team.Enemy)
            return;

        if (trackedEnemies.Contains(health))
            return;

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
        if (Status != QuestStatus.Active)
            return;

        if (health == null)
            return;

        if (!trackedEnemies.Remove(health))
            return;

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
            return;

        if (spawnedEnemies >= TotalEnemies && AliveEnemies <= 0)
            CompleteQuest();
    }

    private void CompleteQuest()
    {
        if (Status != QuestStatus.Active)
            return;

        Status = QuestStatus.Completed;

        OnQuestWon?.Invoke();

        PauseGame();
    }

    private void HandlePlayerDied()
    {
        if (Status != QuestStatus.Active)
            return;

        OnPlayerDied?.Invoke();
        AbortQuest();
    }

    private void AbortQuest()
    {
        UnsubscribeFromEnemySpawner();
        DestroyAllEnemies();
        ClearTrackedEnemies();

        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied -= HandlePlayerDied;
        DestroyPlayer();

        Status = QuestStatus.None;

        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;

        OnAliveCountChanged?.Invoke();
        OnQuestEnded?.Invoke();

        PauseGame();
    }

    private void AbortQuestStart()
    {
        UnsubscribeFromEnemySpawner();
        DestroyAllEnemies();
        ClearTrackedEnemies();

        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied -= HandlePlayerDied;
        DestroyPlayer();

        CurrentQuest = null;
        Status = QuestStatus.None;

        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;

        OnAliveCountChanged?.Invoke();

        PauseGame();
    }

    private void SubscribeToEnemySpawner()
    {
        EnemySpawner.OnEnemySpawned += HandleEnemySpawned;
    }

    private void UnsubscribeFromEnemySpawner()
    {
        EnemySpawner.OnEnemySpawned -= HandleEnemySpawned;
    }

    public void DestroyAllEnemies()
    {
        var enemies = new List<Health>(trackedEnemies);
        foreach (Health enemy in enemies)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
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

    private void SpawnPlayer()
    {
        if (playerPrefab == null) // just for player
        {
            return;
        }

        Vector3 spawnPos = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
        Quaternion spawnRot = playerSpawnPoint != null ? playerSpawnPoint.rotation : Quaternion.identity;

        spawnedPlayer = Instantiate(playerPrefab, spawnPos, spawnRot);
        spawnedPlayer.name = "Player";

        playerLifecycle = spawnedPlayer.GetComponentInChildren<PlayerLifecycle>(true);
        playerHealth = spawnedPlayer.GetComponentInChildren<Health>(true);



        if (enemySpawner != null && playerLifecycle != null)
            enemySpawner.SetPlayerTarget(playerLifecycle.transform);

        playerCamera ??= FindFirstObjectByType<PlayerCamera>(); // cannot put it from inspector
        if (playerCamera != null && playerLifecycle != null)
            playerCamera.SetTarget(playerLifecycle.transform);
    }

    private void DestroyPlayer()
    {
        if (spawnedPlayer != null)
        {
            Destroy(spawnedPlayer);
            spawnedPlayer = null;
        }

        playerLifecycle = null;
        playerHealth = null;
    }

    private void HealPlayer()
    {
        if (playerHealth == null)
            return;

        float missingHealth =
            playerHealth.MaxHealth - playerHealth.CurrentHealth;

        if (missingHealth > 0f)
            playerHealth.Heal(missingHealth);
    }

    private void DestroyAllCardPickups()
    {
        foreach (CardPickup pickup in CardPickup.GetActivePickups())
        {
            if (pickup != null)
                Destroy(pickup.gameObject);
        }
    }

    private void ResumeGame()
    {
        timeScaleManager?.Resume();
    }

    private void PauseGame()
    {
        timeScaleManager?.Pause();
    }

    private void EnablePlayerMovement()
    {
        playerLifecycle?.EnableMovement();
    }

}