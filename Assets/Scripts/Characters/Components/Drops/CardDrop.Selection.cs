using UnityEngine;
using System.Collections.Generic;

public partial class CardDrop
{
    static GameObject lastDroppedCardPrefab;

    bool TryResolveRandomCardPrefab(out GameObject prefab)
    {
        prefab = null;

        if (cardDropPrefabs == null || cardDropPrefabs.Length == 0)
        {
            return false;
        }

        int validPrefabCount = CountValidPrefabs();
        if (validPrefabCount == 0)
        {
            return false;
        }

        List<GameObject> validPrefabs = new List<GameObject>(validPrefabCount);
        for (int i = 0; i < cardDropPrefabs.Length; i++)
        {
            GameObject currentPrefab = cardDropPrefabs[i];
            if (currentPrefab != null)
            {
                validPrefabs.Add(currentPrefab);
            }
        }

        if (validPrefabs.Count == 0)
        {
            return false;
        }

        if (validPrefabs.Count > 1 && lastDroppedCardPrefab != null)
        {
            validPrefabs.RemoveAll(candidate => candidate == lastDroppedCardPrefab);
            if (validPrefabs.Count == 0)
            {
                for (int i = 0; i < cardDropPrefabs.Length; i++)
                {
                    GameObject currentPrefab = cardDropPrefabs[i];
                    if (currentPrefab != null)
                    {
                        validPrefabs.Add(currentPrefab);
                    }
                }
            }
        }

        int randomValidIndex = Random.Range(0, validPrefabs.Count);
        prefab = validPrefabs[randomValidIndex];
        lastDroppedCardPrefab = prefab;
        return true;
    }

    int CountValidPrefabs()
    {
        int validCount = 0;
        for (int i = 0; i < cardDropPrefabs.Length; i++)
        {
            if (cardDropPrefabs[i] != null)
            {
                validCount++;
            }
        }

        return validCount;
    }
}
