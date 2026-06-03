using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
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
        if (enemyPrefab == null)
            return false;

        if (spawnPoints == null || spawnPoints.Count == 0)
            spawnPoints = new List<Transform> { transform };

        enemiesToSpawn = amount;
        enemiesSpawned = 0;
        spawnTimer = 0f;
        return true;
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
                return;
        }

        Transform randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

        GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        newEnemy.name = $"Enemy_{enemiesSpawned + 1}";

        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
        enemyMovement?.SetTarget(playerTarget);

        Health health = newEnemy.GetComponent<Health>();
        if (health != null)
        {
            OnEnemySpawned?.Invoke(health);
        }
    }

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
    }

    private void ResolvePlayerTarget()
    {
        if (playerTarget != null)
            return;

        PlayerLifecycle lifecycle = FindFirstObjectByType<PlayerLifecycle>();
        if (lifecycle != null)
        {
            playerTarget = lifecycle.transform;
            return;
        }
    }
}

