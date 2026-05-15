using System.Collections.Generic;
using UnityEngine;

public class CardDropSelector
{
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

        // Use a scene registry object to keep drop state 
        CardDropRegistry registry = Object.FindObjectOfType<CardDropRegistry>();
        if (registry == null)
        {
            GameObject go = new GameObject("CardDropRegistry");
            registry = go.AddComponent<CardDropRegistry>();
        }


        validPrefabs.RemoveAll(candidate => registry.IsDropped(candidate));

        if (validPrefabs.Count == 0)
        {
            return false;
        }

        selectedPrefab = validPrefabs[Random.Range(0, validPrefabs.Count)];
        registry.MarkDropped(selectedPrefab);
        return true;
    }
}
