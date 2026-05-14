using System;
using System.Collections.Generic;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    [SerializeField] private List<CardConfig> cards;
    
    private  Dictionary<string, int> cardCountsById = new Dictionary<string, int>();
    private  Dictionary<string, Texture> texturesByCardId = new Dictionary<string, Texture>();

    public IReadOnlyDictionary<string, int> CardCountsById => cardCountsById;
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

    public bool TryGetCardConfig(string cardId, out CardConfig config)
    {
        config = cards != null ? cards.Find(c => c != null && c.CardId == cardId) : null;
        return config != null;
    }

    public bool AddCard(string cardId)
    {
        return AddCard(cardId, null);
    }

    public bool AddCard(string cardId, Texture cardTexture)
    {
        if (string.IsNullOrWhiteSpace(cardId))
        {
            return false;
        }

        cardCountsById.TryGetValue(cardId, out int currentCount);
        cardCountsById[cardId] = currentCount + 1;
        if (cardTexture != null)
        {
            texturesByCardId[cardId] = cardTexture;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetCardCount(string cardId)
    {
        if (string.IsNullOrWhiteSpace(cardId))
        {
            return 0;
        }

        return cardCountsById.TryGetValue(cardId, out int count) ? count : 0;
    }

    public bool TryGetCardTexture(string cardId, out Texture texture)
    {
        texture = null;
        return !string.IsNullOrWhiteSpace(cardId) && texturesByCardId.TryGetValue(cardId, out texture);
    }
}
