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

    private void Awake()
    {
        Instance = this;

        enemySpawner ??= EnemySpawner.Instance;
        playerLifecycle ??= PlayerLifecycle.Instance;
        playerHealth ??= Health.PlayerInstance;

        PauseGame();

        if (playerLifecycle != null)
            playerLifecycle.DisableMovement();
    }

    private void OnEnable()
    {
        SubscribeToPlayerDeath();
    }

    private void OnDisable()
    {
        UnsubscribeFromPlayerDeath();
        UnsubscribeFromEnemySpawner();
        ClearTrackedEnemies();
        DestroyPlayer();

        CurrentQuest = null;
        Status = QuestStatus.None;
        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;
    }

    private void SubscribeToPlayerDeath()
    {
        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied += HandlePlayerDied;
    }

    private void UnsubscribeFromPlayerDeath()
    {
        if (playerLifecycle != null)
            playerLifecycle.OnPlayerDied -= HandlePlayerDied;
    }

    public void StartQuest(QuestConfig quest)
    {
        if (quest == null)
        {
            Debug.LogError("[QuestRunner] StartQuest called with null quest.");
            return;
        }

        Debug.Log($"[QuestRunner] StartQuest called: {quest.Name}, enemies={quest.EnemiesAmount}", this);

        CurrentQuest = quest;
        Status = QuestStatus.Active;

        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = Mathf.Max(0, quest.EnemiesAmount);

        ClearTrackedEnemies();
        SubscribeToEnemySpawner();

        DestroyPlayer();
        SpawnPlayer();
        UpdatePlayerReferences();
        SubscribeToPlayerDeath();

        enemySpawner ??= EnemySpawner.Instance;
        if (enemySpawner == null)
        {
            Debug.LogError("[QuestRunner] enemySpawner is null. Cannot start quest.", this);
            AbortQuestStart();
            return;
        }
        Debug.Log($"[QuestRunner] enemySpawner resolved: {enemySpawner.name}", this);

        if (TotalEnemies <= 0)
        {
            Debug.Log("[QuestRunner] TotalEnemies <= 0, completing quest immediately.", this);
            CompleteQuest();
            return;
        }

        bool started = enemySpawner.SpawnEnemies(quest.EnemiesAmount);
        Debug.Log($"[QuestRunner] SpawnEnemies returned: {started}", this);

        if (!started)
        {
            Debug.LogError("[QuestRunner] enemySpawner.SpawnEnemies failed. Aborting quest start.", this);
            AbortQuestStart();
            return;
        }

        ResumeGame();
        EnablePlayerMovement();

        Debug.Log("[QuestRunner] Quest started successfully. Firing events.", this);
        OnQuestStarted?.Invoke(quest);
        OnAliveCountChanged?.Invoke();
    }

    public void EndQuest()
    {
        HealPlayer();

        DestroyAllCardPickups();
        DestroyAllEnemies();
        DestroyPlayer();

        UnsubscribeFromEnemySpawner();
        ClearTrackedEnemies();
        UnsubscribeFromPlayerDeath();

        OnQuestEnded?.Invoke();

        CurrentQuest = null;
        Status = QuestStatus.None;

        spawnedEnemies = 0;
        AliveEnemies = 0;
        TotalEnemies = 0;

        OnAliveCountChanged?.Invoke();

        PauseGame();
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
        {
            Debug.Log($"[QuestRunner] Quest complete! spawned={spawnedEnemies} total={TotalEnemies} alive={AliveEnemies}");
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        if (Status != QuestStatus.Active)
            return;

        Status = QuestStatus.Completed;

        Debug.Log("[QuestRunner] Firing OnQuestWon event.");
        OnQuestWon?.Invoke();

        PauseGame();
    }

    private void HandlePlayerDied()
    {
        if (Status != QuestStatus.Active)
            return;

        AbortQuest();
    }

    private void AbortQuest()
    {
        UnsubscribeFromEnemySpawner();
        DestroyAllEnemies();
        DestroyPlayer();
        ClearTrackedEnemies();
        UnsubscribeFromPlayerDeath();

        CurrentQuest = null;
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
        DestroyPlayer();
        ClearTrackedEnemies();
        UnsubscribeFromPlayerDeath();

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

    private void DestroyAllEnemies()
    {
        var enemies = new List<Health>(trackedEnemies);
        foreach (Health enemy in enemies)
        {
            if (enemy != null && enemy.gameObject != null)
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
        if (playerPrefab == null)
        {
            Debug.Log("[QuestRunner] playerPrefab not assigned. Using existing player in scene.", this);
            return;
        }

        Vector3 spawnPos = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
        Quaternion spawnRot = playerSpawnPoint != null ? playerSpawnPoint.rotation : Quaternion.identity;

        spawnedPlayer = Instantiate(playerPrefab, spawnPos, spawnRot);
        spawnedPlayer.name = "Player";
        Debug.Log($"[QuestRunner] Spawned player from prefab at {spawnPos}", this);
    }

    private void DestroyPlayer()
    {
        if (spawnedPlayer != null)
        {
            Debug.Log("[QuestRunner] Destroying spawned player.", this);
            Destroy(spawnedPlayer);
            spawnedPlayer = null;
        }
    }

    private void UpdatePlayerReferences()
    {
        playerLifecycle = PlayerLifecycle.Instance;
        playerHealth = Health.PlayerInstance;

        if (enemySpawner != null && playerLifecycle != null)
        {
            enemySpawner.SetPlayerTarget(playerLifecycle.transform);
            Debug.Log($"[QuestRunner] Updated enemySpawner playerTarget to {playerLifecycle.name}", this);
        }

        // Update camera to follow newly spawned player
        PlayerCamera playerCamera = FindObjectOfType<PlayerCamera>();
        if (playerCamera != null && playerLifecycle != null)
        {
            playerCamera.SetTarget(playerLifecycle.transform);
        }

        if (playerLifecycle != null)
            Debug.Log($"[QuestRunner] Player references updated. playerLifecycle={playerLifecycle.name}", this);
        else
            Debug.LogWarning("[QuestRunner] playerLifecycle is null after spawning player.", this);
    }

    private void HealPlayer()
    {
        playerHealth ??= Health.PlayerInstance;
        if (playerHealth == null)
            return;

        float missingHealth =
            playerHealth.MaxHealth - playerHealth.CurrentHealth;

        if (missingHealth > 0f)
            playerHealth.Heal(missingHealth);
    }

    private void DestroyAllCardPickups()
    {
        CardPickup[] pickups =
            FindObjectsByType<CardPickup>(FindObjectsSortMode.None);

        foreach (CardPickup pickup in pickups)
        {
            if (pickup != null)
                Destroy(pickup.gameObject);
        }
    }

    private void ResumeGame()
    {
        Debug.Log($"[QuestRunner] ResumeGame. timeScaleManager={(timeScaleManager != null ? "found" : "null")} Time.timeScale={Time.timeScale}", this);
        timeScaleManager?.Resume();
    }

    private void PauseGame()
    {
        timeScaleManager?.Pause();
    }

    private void EnablePlayerMovement()
    {
        playerLifecycle ??= PlayerLifecycle.Instance;
        if (playerLifecycle != null)
        {
            Debug.Log("[QuestRunner] Enabling player movement.", this);
            playerLifecycle.EnableMovement();
        }
        else
        {
            Debug.LogError("[QuestRunner] playerLifecycle is null. Cannot enable movement.", this);
        }
    }

}