using UnityEngine;
using Configs;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Health))]
public class CardDrop : MonoBehaviour
{
    private const float CardDropWorldY = 1.5f;

    [SerializeField] private CardPickup cardDropPrefab;
    [SerializeField] private CardConfig[] dropConfigs;
    [SerializeField] private CardDropCycleState dropCycleState;

    private IDamageable health;
    private bool hasDropped;

    private void Awake()
    {
        health = GetComponent<IDamageable>();

        if (dropCycleState == null)
            dropCycleState = CardDropCycleState.Instance;

        if (dropCycleState == null)
        {
            GameObject stateObject = new GameObject("CardDropCycleState");
            dropCycleState = stateObject.AddComponent<CardDropCycleState>();
        }

        if (health == null)
        {
            Debug.LogWarning($"[CardDrop] {name} is missing IDamageable. Cards won't drop.", this);
            enabled = false;
        }

        if (cardDropPrefab == null)
        {
            Debug.LogError($"[CardDrop] {name}: cardDropPrefab is not assigned! Assign a CardPickup prefab in the Inspector.", this);
        }

        if (dropConfigs == null || dropConfigs.Length == 0)
        {
            Debug.LogWarning($"[CardDrop] {name}: dropConfigs is empty. No cards will drop. Assign CardConfig ScriptableObjects.", this);
        }
    }

    private void OnEnable()
    {
        health.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
        Debug.Log($"[CardDrop] OnDeath called on {name}. hasDropped={hasDropped}", this);

        if (hasDropped)
        {
            Debug.Log("[CardDrop] Already dropped, skipping.", this);
            return;
        }

        if (cardDropPrefab == null)
        {
            Debug.LogError("[CardDrop] cardDropPrefab is null! Cannot drop card.", this);
            return;
        }

        if (!TrySelectConfig(out CardConfig selectedConfig))
        {
            Debug.Log("[CardDrop] No config selected. Card won't drop.", this);
            return;
        }

        hasDropped = true;
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = CardDropWorldY;

        // Offset slightly so card doesn't spawn inside the player
        Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
        spawnPosition.x += randomOffset.x;
        spawnPosition.z += randomOffset.y;

        CardPickup droppedCard = Instantiate(cardDropPrefab, spawnPosition, Quaternion.identity);
        droppedCard.Initialize(selectedConfig);
        Debug.Log($"[CardDrop] Dropped card: {selectedConfig.Name} at {spawnPosition}", this);
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

        List<CardConfig> cycleCandidates = uniqueConfigs.FindAll(cfg => !dropCycleState.UsedConfigIds.Contains(cfg.GetInstanceID()));
        if (cycleCandidates.Count == 0)
        {
            dropCycleState.UsedConfigIds.Clear();
            cycleCandidates = new List<CardConfig>(uniqueConfigs);
        }

        if (cycleCandidates.Count > 1 && dropCycleState.LastDroppedConfigId != -1)
        {
            cycleCandidates.RemoveAll(cfg => cfg.GetInstanceID() == dropCycleState.LastDroppedConfigId);
        }

        if (cycleCandidates.Count == 0)
        {
            cycleCandidates = new List<CardConfig>(uniqueConfigs);
        }

        selectedConfig = cycleCandidates[Random.Range(0, cycleCandidates.Count)];
        int selectedId = selectedConfig.GetInstanceID();
        dropCycleState.UsedConfigIds.Add(selectedId);
        dropCycleState.LastDroppedConfigId = selectedId;
        return true;
    }
}

