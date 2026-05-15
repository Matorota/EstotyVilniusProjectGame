using System.Collections.Generic;
using UnityEngine;

public class CardDropRegistry : MonoBehaviour
{
    private HashSet<GameObject> droppedPrefabs = new HashSet<GameObject>();

    public bool IsDropped(GameObject prefab)
    {
        return prefab != null && droppedPrefabs.Contains(prefab);
    }

    public void MarkDropped(GameObject prefab)
    {
        if (prefab == null) return;
        droppedPrefabs.Add(prefab);
    }

    public void ResetDroppedPrefabs()
    {
        droppedPrefabs.Clear();
    }
}
