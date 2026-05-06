using UnityEngine;

public partial class DefaultEnemyDamage
{
    private bool TryGetTargetHealth(Collider other, out Health targetHealth)
    {
        targetHealth =
            other.GetComponent<Health>() ??
            other.GetComponentInParent<Health>() ??
            other.GetComponentInChildren<Health>();

        return targetHealth != null;
    }
}
