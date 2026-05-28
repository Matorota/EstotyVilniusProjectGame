using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    public static Action<Health> OnEnemySpawned;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();


    private float spawnTimer = 0f;
    private int enemiesToSpawn = 0;
    private int enemiesSpawned = 0;

    public bool SpawnEnemies(int amount)
    {
        Debug.Log($"[EnemySpawner] SpawnEnemies called. amount={amount} enemyPrefab={(enemyPrefab != null ? enemyPrefab.name : "NULL")} playerTarget={(playerTarget != null ? playerTarget.name : "NULL")}", this);

        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab is null! Assign an enemy prefab in the Inspector.", this);
            return false;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
            spawnPoints = new List<Transform> { transform };

        if (playerTarget == null)
        {
            CharacterMovements player = FindObjectOfType<CharacterMovements>();
            if (player != null)
            {
                playerTarget = player.transform;
                Debug.Log($"[EnemySpawner] Found player: {playerTarget.name}", this);
            }
            else
            {
                Debug.LogError("[EnemySpawner] playerTarget is null and could not find CharacterMovements in scene. Is the player destroyed or inactive?", this);
                return false;
            }
        }

        enemiesToSpawn = amount;
        enemiesSpawned = 0;
        spawnTimer = 0f;
        Debug.Log($"[EnemySpawner] SpawnEnemies setup complete. Will spawn {enemiesToSpawn} enemies.", this);
        return true;
    }
    private void Awake()
    {
        if (Instance == null || Instance.gameObject == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }
    private void Update()
    {
        if (enemiesToSpawn == 0 || enemiesSpawned >= enemiesToSpawn)
            return;

        if (playerTarget == null)
            ResolvePlayerTarget();

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnDelay)
        {
            SpawnOneEnemy();
            spawnTimer = 0f;
            enemiesSpawned++;

            if (enemiesSpawned >= enemiesToSpawn)
                enemiesToSpawn = 0;
        }
    }

    private void SpawnOneEnemy()
    {
        if (playerTarget == null)
        {
            ResolvePlayerTarget();
            if (playerTarget == null)
            {
                Debug.LogError("[EnemySpawner] Cannot spawn enemy: playerTarget is null.", this);
                return;
            }
        }

        Transform randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

        GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        newEnemy.name = $"Enemy_{enemiesSpawned + 1}";

        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            var targetField = typeof(EnemyMovement).GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (targetField != null)
            {
                targetField.SetValue(enemyMovement, playerTarget);
            }
        }

        Health health = newEnemy.GetComponent<Health>();
        if (health != null)
        {
            OnEnemySpawned?.Invoke(health);
        }
    }

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
        Debug.Log($"[EnemySpawner] Player target set to: {(target != null ? target.name : "NULL")}", this);
    }

    private void ResolvePlayerTarget()
    {
        if (playerTarget != null && playerTarget.gameObject != null)
            return;

        CharacterMovements player = FindObjectOfType<CharacterMovements>();
        if (player != null)
        {
            playerTarget = player.transform;
            Debug.Log($"[EnemySpawner] Resolved player target: {playerTarget.name}", this);
            return;
        }

        CharacterMovements[] allPlayers = FindObjectsByType<CharacterMovements>(FindObjectsSortMode.None);
        foreach (CharacterMovements p in allPlayers)
        {
            if (p != null && p.gameObject != null)
            {
                playerTarget = p.transform;
                Debug.Log($"[EnemySpawner] Resolved player target (fallback): {playerTarget.name}", this);
                return;
            }
        }

        Debug.LogError("[EnemySpawner] Could not resolve playerTarget. Player may be destroyed or inactive.", this);
    }
}

