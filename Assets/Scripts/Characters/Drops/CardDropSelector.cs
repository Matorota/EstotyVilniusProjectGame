using System.Collections.Generic;
using UnityEngine;

public class CardDropSelector
{
    private static readonly HashSet<GameObject> droppedPrefabs = new HashSet<GameObject>();

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
            if (prefab != null && !droppedPrefabs.Contains(prefab))
            {
                validPrefabs.Add(prefab);
            }
        }

        // If no valid prefabs remain (all have been dropped before), do not select any
        if (validPrefabs.Count == 0)
        {
            return false;
        }

        selectedPrefab = validPrefabs[Random.Range(0, validPrefabs.Count)];
        droppedPrefabs.Add(selectedPrefab);
        return true;
    }

    // Optional helper to reset tracked drops between levels or sessions
    public static void ResetDroppedPrefabs()
    {
        droppedPrefabs.Clear();
    }
}
