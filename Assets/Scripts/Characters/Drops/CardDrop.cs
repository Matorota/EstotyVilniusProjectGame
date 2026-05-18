using UnityEngine;
using Configs;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Health))]
public class CardDrop : MonoBehaviour
{
    private const float CardDropWorldY = 1.5f;
    private static HashSet<int> usedConfigIds = new();
    private static int lastDroppedConfigId = -1;

    [SerializeField] private CardPickup cardDropPrefab;
    [SerializeField] private CardConfig[] dropConfigs;

    private IDamageable health;
    private bool hasDropped;

    private void Awake()
    {
        health = GetComponent<IDamageable>();

        if (health == null)
        {
            Debug.LogWarning($"{nameof(CardDrop)} on {name} is missing IDamageable.");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += OnDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= OnDeath;
        }
    }

    private void OnDeath()
    {
        if (hasDropped)
        {
            return;
        }

        if (cardDropPrefab == null)
        {
            return;
        }

        if (!TrySelectConfig(out CardConfig selectedConfig))
        {
            return;
        }

        hasDropped = true;
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = CardDropWorldY;
        CardPickup droppedCard = Instantiate(cardDropPrefab, spawnPosition, Quaternion.identity);
        droppedCard.Initialize(selectedConfig);
    }

    private bool TrySelectConfig(out CardConfig selectedConfig)
    {
        selectedConfig = null;
        if (dropConfigs == null || dropConfigs.Length == 0)
        {
            return false;
        }

        List<CardConfig> uniqueConfigs = new();
        HashSet<int> seenIds = new();
        for (int i = 0; i < dropConfigs.Length; i++)
        {
            CardConfig config = dropConfigs[i];
            if (config == null)
            {
                continue;
            }

            int configId = config.GetInstanceID();
            if (seenIds.Add(configId))
            {
                uniqueConfigs.Add(config);
            }
        }

        if (uniqueConfigs.Count == 0)
        {
            return false;
        }

        List<CardConfig> cycleCandidates = uniqueConfigs.FindAll(cfg => !usedConfigIds.Contains(cfg.GetInstanceID()));
        if (cycleCandidates.Count == 0)
        {
            usedConfigIds.Clear();
            cycleCandidates = new List<CardConfig>(uniqueConfigs);
        }

        if (cycleCandidates.Count > 1 && lastDroppedConfigId != -1)
        {
            cycleCandidates.RemoveAll(cfg => cfg.GetInstanceID() == lastDroppedConfigId);
        }

        if (cycleCandidates.Count == 0)
        {
            cycleCandidates = new List<CardConfig>(uniqueConfigs);
        }

        selectedConfig = cycleCandidates[Random.Range(0, cycleCandidates.Count)];
        int selectedId = selectedConfig.GetInstanceID();
        usedConfigIds.Add(selectedId);
        lastDroppedConfigId = selectedId;
        return true;
    }
}
