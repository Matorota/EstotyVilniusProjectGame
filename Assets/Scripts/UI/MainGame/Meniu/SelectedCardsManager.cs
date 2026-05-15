using System;
using System.Collections.Generic;
using Characters.Player.Inventory;
using UnityEngine;

public class SelectedCardsManager : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;
    private List<CardType> selected = new List<CardType>();
    public IReadOnlyList<CardType> Selected => selected;
    public event Action OnSelectedChanged;
    [SerializeField] private int maxSelected = 3;

    private void Awake()
    {
        inventory ??= FindObjectOfType<CardInventory>();
        
    }

    public bool TryEquip(CardType type)
    {
        if (selected.Contains(type))
        {
            Debug.Log($"Already equipped {type}");
            return false;
        }
        if (selected.Count >= maxSelected)
        {
            Debug.Log($"Cannot equip {type}: max selected {maxSelected} reached");
            return false;
        }
        if (inventory == null)
        {
            Debug.LogWarning("No inventory reference in SelectedCardsManager");
            return false;
        }
        if (inventory.GetCardCount(type) <= 0)
        {
            Debug.Log($"No card of type {type} in inventory to equip");
            return false;
        }
        bool removed = inventory.RemoveCard(type);
        if (!removed)
        {
            Debug.Log($"RemoveCard failed for {type}");
            return false;
        }
        selected.Add(type);
        OnSelectedChanged?.Invoke();
        Debug.Log($"Equipped card {type} — selected now: {selected.Count}");
        return true;
    }

    public bool TryUnequip(CardType type)
    {
        if (!selected.Contains(type)) return false;
        selected.Remove(type);
        inventory.AddCard(type);
        OnSelectedChanged?.Invoke();
        return true;
    }

    public void ClearSelected()
    {
        foreach (var t in new List<CardType>(selected))
        {
            TryUnequip(t);
        }
    }
}
