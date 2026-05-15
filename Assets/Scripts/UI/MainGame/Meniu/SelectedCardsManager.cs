using System;
using System.Collections.Generic;
using System.Linq;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;

public class SelectedCardsManager : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;

    private sealed class SelectedEntry
    {
        public CardType Type;
        public CardConfig Config;
        public Texture Texture;
    }

    private List<SelectedEntry> selectedEntries = new List<SelectedEntry>();

    // Public info struct for UI/other systems
    public struct SelectedCardInfo
    {
        public CardType Type;
        public CardConfig Config;
        public Texture Texture;
    }

    // expose selected types for backwards compatibility
    public IReadOnlyList<CardType> Selected => selectedEntries.ConvertAll(e => e.Type);
    public event Action OnSelectedChanged;
    [SerializeField] private int maxSelected = 3;

    private void Awake()
    {
        inventory ??= FindObjectOfType<CardInventory>();
    }

    // Equip by type, default behaviour — removes oldest instance
    public bool TryEquip(CardType type)
    {
        return TryEquip(type, 0);
    }

    // Equip a specific instance (index) so UI-picked prefab stays consistent
    public bool TryEquip(CardType type, int instanceIndex)
    {
        if (inventory == null)
        {
            Debug.LogWarning("No inventory reference in SelectedCardsManager");
            return false;
        }

        // Preview the config at the requested index to perform uniqueness checks before removal
        CardConfig previewConfig = inventory.GetCardConfigAtIndex(type, instanceIndex);

        // block if same type already selected
        if (selectedEntries.Any(e => e.Type == type))
        {
            Debug.Log($"Already equipped card with type {type}");
            return false;
        }

        // block if same name already selected (name is the uniqueness key)
        if (previewConfig != null && selectedEntries.Any(e => e.Config != null && e.Config.Name == previewConfig.Name))
        {
            Debug.Log($"Already equipped card with name '{previewConfig.Name}'");
            return false;
        }

        if (selectedEntries.Count >= maxSelected)
        {
            Debug.Log($"Cannot equip {type}: max selected {maxSelected} reached");
            return false;
        }

        if (inventory.GetCardCount(type) <= 0)
        {
            Debug.Log($"No card of type {type} in inventory to equip");
            return false;
        }

        if (!inventory.RemoveCardAtIndex(type, instanceIndex, out CardConfig cfg, out Texture tex))
        {
            Debug.Log($"RemoveCardAtIndex failed for {type} index {instanceIndex}");
            return false;
        }

        selectedEntries.Add(new SelectedEntry { Type = type, Config = cfg, Texture = tex });
        OnSelectedChanged?.Invoke();
        Debug.Log($"Equipped card {type} ('{cfg?.Name}') — selected now: {selectedEntries.Count}");
        return true;
    }

    public bool TryUnequip(CardType type)
    {
        var entry = selectedEntries.Find(e => e.Type == type);
        if (entry == null) return false;
        selectedEntries.Remove(entry);
        // restore exact instance if we have config info, otherwise fallback to type-only add
        if (entry.Config != null)
        {
            inventory.AddCard(entry.Config, entry.Texture);
        }
        else
        {
            inventory.AddCard(type);
        }
        OnSelectedChanged?.Invoke();
        return true;
    }

    public void ClearSelected()
    {
        foreach (var e in new List<SelectedEntry>(selectedEntries))
        {
            TryUnequip(e.Type);
        }
    }

    // Provide detailed selected info for UI
    public List<SelectedCardInfo> GetSelectedCards()
    {
        return selectedEntries.Select(e => new SelectedCardInfo { Type = e.Type, Config = e.Config, Texture = e.Texture }).ToList();
    }
}
