using System;
using System.Collections.Generic;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;

public class CardInventoryUi : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject[] cardPrefabs;

    private Dictionary<CardType, GameObject> prefabByCardType = new Dictionary<CardType, GameObject>();
    private Dictionary<CardType, List<CardEntry>> entriesByCardType = new Dictionary<CardType, List<CardEntry>>();
    private HashSet<CardType> missingPrefabWarnings = new HashSet<CardType>();

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
        prefabByCardType.Clear();

        if (cardPrefabs == null)
        {
            return;
        }

        foreach (GameObject cardPrefab in cardPrefabs)
        {
            if (cardPrefab == null) continue;

            CardWidget widget = cardPrefab.GetComponent<CardWidget>() ?? cardPrefab.GetComponentInChildren<CardWidget>(true);
            if (widget == null || widget.Config == null) continue;

            prefabByCardType[widget.Config.Type] = cardPrefab;
        }
    }

    private void RefreshEntries()
    {
        IReadOnlyDictionary<CardType, int> cardCounts = inventory.CardCountsByType;
        int visibleIndex = 0;

        foreach (var pair in cardCounts)
        {
            CardType type = pair.Key;
            int count = pair.Value;
            if (count <= 0) continue;

            if (!entriesByCardType.TryGetValue(type, out List<CardEntry> list))
            {
                list = new List<CardEntry>();
                entriesByCardType[type] = list;
            }

            while (list.Count < count)
            {
                CardEntry newEntry = CreateEntry(type);
                if (newEntry != null)
                {
                    list.Add(newEntry);
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                CardEntry entry = list[i];
                if (entry == null || entry.Root == null) continue;

                if (i < count)
                {
                    ShowEntry(entry, visibleIndex);
                    visibleIndex++;
                }
                else
                {
                    entry.Root.SetActive(false);
                }
            }
        }

        foreach (var kv in entriesByCardType)
        {
            CardType type = kv.Key;
            if (!cardCounts.TryGetValue(type, out int cnt) || cnt <= 0)
            {
                foreach (var entry in kv.Value)
                {
                    if (entry != null && entry.Root != null)
                        entry.Root.SetActive(false);
                }
            }
        }
    }

    private void ShowEntry(CardEntry entry, int visibleIndex)
    {
        entry.Root.SetActive(true);
        entry.Root.transform.SetSiblingIndex(visibleIndex);
    }

    private CardEntry CreateEntry(CardType type)
    {
        GameObject uiPrefab = ResolveUiPrefab(type);
        if (uiPrefab == null)
        {
            if (missingPrefabWarnings.Add(type))
            {
                Debug.LogWarning($"{nameof(CardInventoryUi)}: No UI prefab found for card type '{type}'. Add matching prefab to Card Prefabs.");
            }

            return null;
        }

        GameObject root = Instantiate(uiPrefab, cardsContainer);
        root.name = $"Card_{type}_{Guid.NewGuid():N}";

        CardWidget widget = root.GetComponent<CardWidget>() ?? root.GetComponentInChildren<CardWidget>(true);
        if (widget != null)
        {
            CardConfig config = inventory.GetCardConfig(type);
            if (config != null)
            {
                widget.SetConfig(config);
            }
        }

        return new CardEntry
        {
            Root = root,
            Widget = widget
        };
    }

    private GameObject ResolveUiPrefab(CardType type)
    {
        if (prefabByCardType.TryGetValue(type, out GameObject prefab))
        {
            return prefab;
        }

        return null;
    }
}

