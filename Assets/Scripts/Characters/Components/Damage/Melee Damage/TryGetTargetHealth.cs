using UnityEngine;

public partial class DefaultEnemyDamage
{
    bool TryGetTargetHealth(Collider other, out Health targetHealth)
    {
        targetHealth =
            other.GetComponent<Health>() ??
            other.GetComponentInParent<Health>() ??
            other.GetComponentInChildren<Health>();

        return targetHealth != null;
    }
}
