using System.Collections.Generic;
using UnityEngine;

public class CardDropSelector
{
    private static GameObject lastDroppedCardPrefab;

    public bool TrySelectPrefab(GameObject[] prefabs, out GameObject selectedPrefab)
    {
        selectedPrefab = null;

        if (prefabs == null || prefabs.Length == 0)
        {
            return false;
        }

        List<GameObject> validPrefabs = new List<GameObject>(prefabs.Length);
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            if (prefab != null)
            {
                validPrefabs.Add(prefab);
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
                for (int i = 0; i < prefabs.Length; i++)
                {
                    GameObject prefab = prefabs[i];
                    if (prefab != null)
                    {
                        validPrefabs.Add(prefab);
                    }
                }
            }
        }

        selectedPrefab = validPrefabs[Random.Range(0, validPrefabs.Count)];
        lastDroppedCardPrefab = selectedPrefab;
        return true;
    }
}
