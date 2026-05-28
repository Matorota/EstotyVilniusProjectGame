using System.Collections.Generic;
using UnityEngine;

public class CardDropCycleState : MonoBehaviour
{
    public static CardDropCycleState Instance { get; private set; }

    public HashSet<int> UsedConfigIds { get; } = new();
    public int LastDroppedConfigId { get; set; } = -1;

    private void Awake()
    {
        if (Instance == null || Instance.gameObject == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
