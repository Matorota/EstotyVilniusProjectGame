using System.Collections.Generic;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;

public class CardInventoryUi : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject[] cardPrefabs;

    private  Dictionary<string, GameObject> prefabByCardId = new Dictionary<string, GameObject>();
    private  Dictionary<CardType, GameObject> prefabByCardType = new Dictionary<CardType, GameObject>();
    private  Dictionary<string, CardEntry> entriesByCardId = new Dictionary<string, CardEntry>();
    private  HashSet<string> missingPrefabWarnings = new HashSet<string>();

    private sealed class CardEntry
    {
        public GameObject Root;
        public CardWidget Widget;
    }

    private void Awake()
    {
        inventory ??= FindObjectOfType<CardInventory>();
        cardsContainer ??= transform;
        CacheCardPrefabs();
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (inventory == null || cardsContainer == null) return;
        RefreshEntries();
    }

    private void CacheCardPrefabs()
    {
        prefabByCardId.Clear();
        prefabByCardType.Clear();

        if (cardPrefabs == null)
        {
            return;
        }

        foreach (GameObject cardPrefab in cardPrefabs)
        {
            if (cardPrefab == null) continue;

            CardPickup pickup = cardPrefab.GetComponent<CardPickup>() ?? cardPrefab.GetComponentInChildren<CardPickup>(true);
            if (pickup != null && !string.IsNullOrWhiteSpace(pickup.CardId))
            {
                prefabByCardId[pickup.CardId] = cardPrefab;
            }

            CardWidget widget = cardPrefab.GetComponent<CardWidget>() ?? cardPrefab.GetComponentInChildren<CardWidget>(true);
            if (widget == null) continue;

            if (widget.Config != null && !string.IsNullOrWhiteSpace(widget.Config.CardId))
            {
                prefabByCardId[widget.Config.CardId] = cardPrefab;
            }

            prefabByCardType[widget.CardType] = cardPrefab;
        }
    }

    private void RefreshEntries()
    {
        IReadOnlyDictionary<string, int> cardCounts = inventory.CardCountsById;
        int visibleIndex = 0;

        foreach (var pair in cardCounts)
        {
            string cardId = pair.Key;
            int count = pair.Value;
            if (count <= 0) continue;

            if (!entriesByCardId.TryGetValue(cardId, out CardEntry entry))
            {
                entry = CreateEntry(cardId);
                entriesByCardId[cardId] = entry;
            }

            if (entry == null || entry.Root == null) continue;

            ShowEntry(entry, visibleIndex);
            visibleIndex++;
        }

        UpdateEntryVisibility(cardCounts);
    }

    private void ShowEntry(CardEntry entry, int visibleIndex)
    {
        entry.Root.SetActive(true);
        entry.Root.transform.SetSiblingIndex(visibleIndex);
    }

    private void UpdateEntryVisibility(IReadOnlyDictionary<string, int> cardCounts)
    {
        foreach (var pair in entriesByCardId)
        {
            string cardId = pair.Key;
            CardEntry entry = pair.Value;
            bool hasCard = cardCounts.TryGetValue(cardId, out int count) && count > 0;
            if (entry != null && entry.Root != null)
            {
                entry.Root.SetActive(hasCard);
            }
        }
    }

    private CardEntry CreateEntry(string cardId)
    {
        GameObject uiPrefab = ResolveUiPrefab(cardId);
        if (uiPrefab == null)
        {
            if (missingPrefabWarnings.Add(cardId))
            {
                Debug.LogWarning($"{nameof(CardInventoryUi)}: No UI prefab found for cardId '{cardId}'. Add matching prefab to Card Prefabs.");
            }

            return null;
        }

        GameObject root = Instantiate(uiPrefab, cardsContainer);
        root.name = $"Card_{cardId}";

        CardWidget widget = root.GetComponent<CardWidget>() ?? root.GetComponentInChildren<CardWidget>(true);
        if (widget != null && inventory.TryGetCardConfig(cardId, out CardConfig config))
        {
            widget.SetConfig(config);
        }

        return new CardEntry
        {
            Root = root,
            Widget = widget
        };
    }

    private GameObject ResolveUiPrefab(string cardId)
    {
        if (prefabByCardId.TryGetValue(cardId, out GameObject directPrefab))
        {
            return directPrefab;
        }

        if (inventory != null && inventory.TryGetCardConfig(cardId, out CardConfig config) && config != null)
        {
            if (prefabByCardType.TryGetValue(config.Type, out GameObject typePrefab))
            {
                return typePrefab;
            }
        }

        return null;
    }
}
