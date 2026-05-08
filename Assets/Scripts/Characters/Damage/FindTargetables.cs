using Characters.Health;
using UnityEngine;

public class FindTargetables : MonoBehaviour
{
    private const float FaceDotThreshold = 0.8f;

    public IDamageable FindTarget(Transform origin, IDamageable selfDamageable, float range)
    {
        IDamageable best = null;
        float bestDistance = float.MaxValue;

        foreach (Collider hit in Physics.OverlapSphere(origin.position, range, ~0, QueryTriggerInteraction.Ignore))
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target == null || !IsHostile(selfDamageable, target))
            {
                continue;
            }

            Vector3 offset = Vector3.ProjectOnPlane(((Component)target).transform.position - origin.position, Vector3.up);
            float sqrDistance = offset.sqrMagnitude;
            if (sqrDistance > range * range || sqrDistance >= bestDistance)
            {
                continue;
            }

            bestDistance = sqrDistance;
            best = target;
        }

        return best;
    }

    public bool IsTargetValid(Transform origin, IDamageable selfDamageable, IDamageable target, float range) // without it would attack no matter what range
    {
        if (target == null || target.CurrentHealth <= 0f || !IsHostile(selfDamageable, target))
        {
            return false;
        }

        Vector3 offset = Vector3.ProjectOnPlane(((Component)target).transform.position - origin.position, Vector3.up);
        return offset.sqrMagnitude <= range * range;
    }

    public bool IsFacingTarget(Transform origin, IDamageable target)
    {
        Vector3 offset = Vector3.ProjectOnPlane(((Component)target).transform.position - origin.position, Vector3.up);
        return offset.sqrMagnitude <= 0.0001f || Vector3.Dot(origin.forward, offset.normalized) >= FaceDotThreshold;
    }

    public bool IsHostile(IDamageable selfDamageable, IDamageable target)
    {
        return target != null && (selfDamageable == null || target.Team != selfDamageable.Team);
    }
}
