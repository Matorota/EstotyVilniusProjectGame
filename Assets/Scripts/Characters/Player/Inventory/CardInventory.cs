using System;
using System.Collections.Generic;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    private Dictionary<string, int> cardCountsById = new Dictionary<string, int>(); // aray with no order

    public IReadOnlyDictionary<string, int> CardCountsById => cardCountsById;
    public event Action OnInventoryChanged;

    public bool AddCard(string cardId)
    {
        if (string.IsNullOrWhiteSpace(cardId))
        {
            return false;
        }

        cardCountsById.TryGetValue(cardId, out int currentCount);
        cardCountsById[cardId] = currentCount + 1;
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
}
