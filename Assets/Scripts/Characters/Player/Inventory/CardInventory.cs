using System;
using System.Collections.Generic;
using System.Linq;
using Characters.Player.Inventory;
using UnityEngine;

public class CardInventory : MonoBehaviour // sadly but i cannot live without statistics here doesn't store cards if no static
{
    private static readonly List<CardModel> collectedCards = new();
    private static readonly List<CardModel> equippedCards = new();
    private static readonly Dictionary<CardType, List<CardModel>> cardModelsByType = new();
    private static readonly List<CardModel> currentQuestCards = new();

    public event Action OnInventoryChanged;

    public static void ResetInventory()
    {
        collectedCards.Clear();
        equippedCards.Clear();
        cardModelsByType.Clear();
        currentQuestCards.Clear();
    }

    public static void BeginQuestCardTracking()
    {
        currentQuestCards.Clear();
    }

    public static void ResetCurrentQuestCards()
    {
        foreach (CardModel model in currentQuestCards)
        {
            if (model == null || model.config == null)
                continue;

            model.MarkUnequipped();
            equippedCards.Remove(model);
            collectedCards.Remove(model);

            if (cardModelsByType.TryGetValue(model.config.Type, out List<CardModel> models))
            {
                models.Remove(model);
            }
        }

        currentQuestCards.Clear();
    }

    public bool Collect(CardModel model)
    {
        if (model == null || model.config == null)
        {
            return false;
        }

        CardType type = model.config.Type;
        if (!cardModelsByType.TryGetValue(type, out List<CardModel> models))
        {
            models = new List<CardModel>();
            cardModelsByType[type] = models;
        }

        model.MarkCollected();
        models.Add(model);
        collectedCards.Add(model);
        currentQuestCards.Add(model);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool Equip(CardModel model)
    {
        if (model == null || model.config == null || model.isEquipped || !Contains(model))
        {
            return false;
        }

        model.MarkEquipped();
        if (!equippedCards.Contains(model))
        {
            equippedCards.Add(model);
        }
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool Unequip(CardModel model)
    {
        if (model == null || model.config == null || !model.isEquipped || !Contains(model))
        {
            return false;
        }

        model.MarkUnequipped();
        equippedCards.Remove(model);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public List<CardModel> GetCollectedCards()
    {
        return new List<CardModel>(collectedCards);
    }

    public List<CardModel> GetUnequippedCards()
    {
        return collectedCards.Where(model => !model.isEquipped).ToList();
    }

    public List<CardModel> GetEquippedCards()
    {
        return new List<CardModel>(equippedCards);
    }

    private bool Contains(CardModel model)
    {
        if (model == null || model.config == null)
        {
            return false;
        }

        return cardModelsByType.TryGetValue(model.config.Type, out List<CardModel> models) && models.Contains(model);
    }
}
