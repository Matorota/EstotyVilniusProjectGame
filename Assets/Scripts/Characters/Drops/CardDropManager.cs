using System.Collections.Generic;
using UnityEngine;

public static class CardDropManager
{
    private static readonly List<CardPickup> spawnedCards = new List<CardPickup>();

    public static void Register(CardPickup card)
    {
        if (card == null) return;
        if (!spawnedCards.Contains(card))
            spawnedCards.Add(card);
    }

    public static void Unregister(CardPickup card)
    {
        if (card == null) return;
        spawnedCards.Remove(card);
    }

    public static void ClearAll()
    {
        for (int i = spawnedCards.Count - 1; i >= 0; i--)
        {
            var c = spawnedCards[i];
            if (c != null)
            {
                GameObject.Destroy(c.gameObject);
            }
        }
        spawnedCards.Clear();
    }
}
