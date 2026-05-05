using UnityEngine;

public partial class CardDrop
{
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

        int randomValidIndex = Random.Range(0, validPrefabCount);
        for (int i = 0; i < cardDropPrefabs.Length; i++)
        {
            GameObject currentPrefab = cardDropPrefabs[i];
            if (currentPrefab == null)
            {
                continue;
            }

            if (randomValidIndex == 0)
            {
                prefab = currentPrefab;
                return true;
            }

            randomValidIndex--;
        }

        return false;
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
