using System.Collections.Generic;
using UnityEngine;

public class CardDropCycleState : MonoBehaviour
{
    public HashSet<int> UsedConfigIds { get; } = new();
    public int LastDroppedConfigId { get; set; } = -1;
}
