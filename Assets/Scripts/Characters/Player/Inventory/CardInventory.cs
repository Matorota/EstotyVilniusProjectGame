using System;
using System.Collections.Generic;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    [SerializeField] private List<CardConfig> cards;
    private Dictionary<CardType, List<CardConfig>> instanceConfigsByType = new Dictionary<CardType, List<CardConfig>>();
    private Dictionary<CardType, List<Texture>> instanceTexturesByType = new Dictionary<CardType, List<Texture>>();
    private Dictionary<CardType, int> cardCountsByType = new Dictionary<CardType, int>();

    public IReadOnlyDictionary<CardType, int> CardCountsByType => cardCountsByType;
    public event Action OnInventoryChanged;

    public CardConfig GetCardConfig(CardType type)
    {
        if (instanceConfigsByType.TryGetValue(type, out var list) && list != null && list.Count > 0)
        {
            return list[0];
        }
        return cards == null ? null : cards.Find(c => c != null && c.Type == type);
    }

    public bool AddCard(CardConfig config, Texture cardTexture)
    {
        if (config == null)
        {
            return false;
        }

        CardType type = config.Type;
        if (!instanceConfigsByType.TryGetValue(type, out var cfgList))
        {
            cfgList = new List<CardConfig>();
            instanceConfigsByType[type] = cfgList;
        }
        cfgList.Add(config);

        if (cardTexture != null)
        {
            if (!instanceTexturesByType.TryGetValue(type, out var texList))
            {
                texList = new List<Texture>();
                instanceTexturesByType[type] = texList;
            }
            texList.Add(cardTexture);
        }
        cardCountsByType.TryGetValue(type, out int cur);
        cardCountsByType[type] = cur + 1;

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool AddCard(CardType type)
    {
        return AddCard(type, null);
    }

    public bool AddCard(CardType type, Texture cardTexture)
    {
        CardConfig found = cards != null ? cards.Find(c => c != null && c.Type == type) : null;
        if (found != null)
        {
            return AddCard(found, cardTexture);
        }

        cardCountsByType.TryGetValue(type, out int currentCount);
        cardCountsByType[type] = currentCount + 1;
        if (cardTexture != null)
        {
            if (!instanceTexturesByType.TryGetValue(type, out var texList))
            {
                texList = new List<Texture>();
                instanceTexturesByType[type] = texList;
            }
            texList.Add(cardTexture);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetCardCount(CardType type)
    {
        return cardCountsByType.TryGetValue(type, out int count) ? count : 0;
    }

    public CardConfig GetCardConfigAtIndex(CardType type, int index)
    {
        if (instanceConfigsByType.TryGetValue(type, out var list) && list != null && index >= 0 && index < list.Count)
        {
            return list[index];
        }
        return cards != null ? cards.Find(c => c != null && c.Type == type) : null;
    }

    public bool TryGetCardTextureAtIndex(CardType type, int index, out Texture texture)
    {
        texture = null;
        if (instanceTexturesByType.TryGetValue(type, out var list) && list != null && index >= 0 && index < list.Count)
        {
            texture = list[index];
            return texture != null;
        }
        return false;
    }

    public bool RemoveCard(CardType type)
    {
        return RemoveCardAtIndex(type, 0, out _, out _);
    }

    public bool RemoveCardAtIndex(CardType type, int index, out CardConfig removedConfig, out Texture removedTexture)
    {
        removedConfig = null;
        removedTexture = null;

        if (index < 0) return false;

        if (instanceConfigsByType.TryGetValue(type, out var cfgList) && cfgList != null && cfgList.Count > 0)
        {
            if (index >= cfgList.Count) return false;

            removedConfig = cfgList[index];
            cfgList.RemoveAt(index);
            if (cfgList.Count == 0) instanceConfigsByType.Remove(type);

            if (instanceTexturesByType.TryGetValue(type, out var texList) && texList != null && texList.Count > index)
            {
                removedTexture = texList[index];
                texList.RemoveAt(index);
                if (texList.Count == 0) instanceTexturesByType.Remove(type);
            }

            if (cardCountsByType.TryGetValue(type, out int cur) && cur > 0)
            {
                cardCountsByType[type] = cur - 1;
                if (cardCountsByType[type] <= 0)
                {
                    cardCountsByType.Remove(type);
                }
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        if (cardCountsByType.TryGetValue(type, out int current) && current > 0)
        {
            cardCountsByType[type] = current - 1;
            if (cardCountsByType[type] <= 0)
            {
                cardCountsByType.Remove(type);
                instanceTexturesByType.Remove(type);
            }
            OnInventoryChanged?.Invoke();
            return true;
        }

        return false;
    }
}
