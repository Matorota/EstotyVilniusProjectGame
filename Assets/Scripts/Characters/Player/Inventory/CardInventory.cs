using System;
using System.Collections.Generic;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    [SerializeField] private List<CardConfig> cards;

    private Dictionary<CardType, int> cardCountsByType = new Dictionary<CardType, int>();
    private Dictionary<CardType, Texture> texturesByType = new Dictionary<CardType, Texture>();

    public IReadOnlyDictionary<CardType, int> CardCountsByType => cardCountsByType;
    public event Action OnInventoryChanged;

    public CardConfig GetCardConfig(CardType type) // use to setup card widget
    {
        return cards != null ? cards.Find(c => c != null && c.Type == type) : null;
    }

    public CardConfig GetSpeedUpCardConfig()
    {
        return GetCardConfig(CardType.SpeedUp);
    }

    public CardConfig GetDamageCardConfig()
    {
        return GetCardConfig(CardType.Damage);
    }

    public CardConfig GetHealthCardConfig()
    {
        return GetCardConfig(CardType.Health);
    }

    public bool TryGetCardConfig(CardType type, out CardConfig config)
    {
        config = cards != null ? cards.Find(c => c != null && c.Type == type) : null;
        return config != null;
    }

    public bool AddCard(CardType type)
    {
        return AddCard(type, null);
    }

    public bool AddCard(CardType type, Texture cardTexture)
    {
        cardCountsByType.TryGetValue(type, out int currentCount);
        cardCountsByType[type] = currentCount + 1;
        if (cardTexture != null)
        {
            texturesByType[type] = cardTexture;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetCardCount(CardType type)
    {
        return cardCountsByType.TryGetValue(type, out int count) ? count : 0;
    }

    public bool TryGetCardTexture(CardType type, out Texture texture)
    {
        texture = null;
        return texturesByType.TryGetValue(type, out texture);
    }
}
