using System;
using System.Collections.Generic;
using System.Linq;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;

public class SelectedCardsManager : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;
    [SerializeField] private PlayerStats stats;

    public struct SelectedCardInfo
    {
        public CardModel Model;
        public CardType Type;
        public CardConfig Config;
        public bool IsActive;
    }

    public event Action OnSelectedChanged;
    [SerializeField] private int maxSelected = 3;

    private void OnEnable()
    {
        inventory.OnInventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= HandleInventoryChanged;
    }

    public bool TryEquip(CardModel model)
    {
        if (model.isEquipped)
        {
            return false;
        }

        if (inventory.GetEquippedCards().Count >= maxSelected)
        {
            return false;
        }

        if (!inventory.Equip(model))
        {
            return false;
        }

        OnSelectedChanged?.Invoke();
        return true;
    }

    public bool TryUnequip(CardModel model)
    {
        if (!inventory.Unequip(model))
        {
            return false;
        }

        model.MarkInactive();
        OnSelectedChanged?.Invoke();
        return true;
    }

    public void ClearSelected()
    {
        foreach (CardModel model in inventory.GetEquippedCards().ToList())
        {
            TryUnequip(model);
        }
    }

    public bool TryUseByIndex(int index)
    {
        List<CardModel> equippedCards = inventory.GetEquippedCards();
        if (index < 0 || index >= equippedCards.Count) return false;
        CardModel model = equippedCards[index];
        if (model.isActive) return false;

        stats.ApplyCardEffect(model.config);
        if (isActiveAndEnabled)
        {
            StartCoroutine(ActivateForDuration(model, model.config.Duration));
        }
        else if (stats.isActiveAndEnabled)
        {
            stats.StartCoroutine(ActivateForDuration(model, model.config.Duration));
        }
        else
        {
            return false;
        }

        OnSelectedChanged?.Invoke();
        return true;
    }

    private System.Collections.IEnumerator ActivateForDuration(CardModel model, float duration)
    {
        model.MarkActive();
        OnSelectedChanged?.Invoke();
        yield return new WaitForSeconds(Mathf.Max(0f, duration));
        model.MarkInactive();
        OnSelectedChanged?.Invoke();
    }

    public List<SelectedCardInfo> GetSelectedCards()
    {
        return inventory
            .GetEquippedCards()
            .Select(model => new SelectedCardInfo
            {
                Model = model,
                Type = model.config.Type,
                Config = model.config,
                IsActive = model.isActive
            })
            .ToList();
    }

    private void HandleInventoryChanged()
    {
        foreach (CardModel model in inventory.GetCollectedCards())
        {
            if (model != null && !model.isEquipped && model.isActive)
            {
                model.MarkInactive();
            }
        }

        OnSelectedChanged?.Invoke();
    }
}

