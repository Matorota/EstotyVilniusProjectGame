using Characters.Health;
using UnityEngine;

public struct DefaultEnemyDamageTargetContext
{
    public int TargetId;
    public Transform TargetTransform;
    public IDamageable TargetDamageable;
    public float CurrentTime;
    public float TargetDamageMultiplier;
}
