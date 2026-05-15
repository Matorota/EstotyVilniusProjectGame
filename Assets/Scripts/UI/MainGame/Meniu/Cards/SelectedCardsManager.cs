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
        public bool IsActive;
    }

    private List<SelectedEntry> selectedEntries = new List<SelectedEntry>();

    // Public info struct for UI/other systems
    public struct SelectedCardInfo
    {
        public CardType Type;
        public CardConfig Config;
        public Texture Texture;
        public bool IsActive;
    }

    public event Action OnSelectedChanged;
    [SerializeField] private int maxSelected = 3;
    private PlayerStats stats;

    private void Awake()
    {
        inventory ??= FindObjectOfType<CardInventory>();
        stats ??= FindObjectOfType<PlayerStats>();
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
            return false;
        }

        // block if same name already selected (name is the uniqueness key)
        if (previewConfig != null && selectedEntries.Any(e => e.Config != null && e.Config.Name == previewConfig.Name))
        {
            return false;
        }

        if (selectedEntries.Count >= maxSelected)
        {
            return false;
        }

        if (inventory.GetCardCount(type) <= 0)
        {
            return false;
        }

        if (!inventory.RemoveCardAtIndex(type, instanceIndex, out CardConfig cfg, out Texture tex))
        {
            return false;
        }

        selectedEntries.Add(new SelectedEntry { Type = type, Config = cfg, Texture = tex, IsActive = false });
        OnSelectedChanged?.Invoke();
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

    // Use by slot index (0-based) — maps to Ability 1/2/3. Keeps ordering of selected entries.
    public bool TryUseByIndex(int index)
    {
        if (index < 0 || index >= selectedEntries.Count) return false;
        var entry = selectedEntries[index];
        if (entry == null) return false;
        if (entry.IsActive) return false; // already active

        if (stats == null)
        {
            return false;
        }

        if (entry.Config == null)
        {
            return false;
        }

        stats.ApplyCardEffect(entry.Config);
        StartCoroutine(ActivateEntryForDuration(entry, entry.Config.Duration));
        OnSelectedChanged?.Invoke();
        return true;
    }

    private System.Collections.IEnumerator ActivateEntryForDuration(SelectedEntry entry, float duration)
    {
        entry.IsActive = true;
        OnSelectedChanged?.Invoke();
        yield return new WaitForSeconds(Mathf.Max(0f, duration));
        entry.IsActive = false;
        OnSelectedChanged?.Invoke();
    }

    // Provide detailed selected info for UI
    public List<SelectedCardInfo> GetSelectedCards()
    {
        return selectedEntries.Select(e => new SelectedCardInfo { Type = e.Type, Config = e.Config, Texture = e.Texture, IsActive = e.IsActive }).ToList();
    }
}
