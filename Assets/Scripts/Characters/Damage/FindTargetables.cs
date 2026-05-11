using UnityEngine;

public class FindTargetables : MonoBehaviour
{
    private const float FaceDotThreshold = 0.8f;

    public static IDamageable FindTarget(Transform origin, IDamageable selfDamageable, float range)
    {
        IDamageable best = null;
        float bestDistance = float.MaxValue;
        float rangeSqr = range * range;

        foreach (Collider hit in Physics.OverlapSphere(origin.position, range, ~0, QueryTriggerInteraction.Ignore))
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target == null || !IsHostile(selfDamageable, target))
            {
                continue;
            }

            if (!TryGetTargetTransform(target, out Transform targetTransform))
            {
                continue;
            }

            Vector3 offset = Vector3.ProjectOnPlane(targetTransform.position - origin.position, Vector3.up);
            float sqrDistance = offset.sqrMagnitude;
            if (sqrDistance > rangeSqr || sqrDistance >= bestDistance)
            {
                continue;
            }

            bestDistance = sqrDistance;
            best = target;
        }

        return best;
    }

    public static bool IsTargetValid(Transform origin, IDamageable selfDamageable, IDamageable target, float range)
    {
        if (target == null || target.CurrentHealth <= 0f || !IsHostile(selfDamageable, target))
        {
            return false;
        }

        if (!TryGetTargetTransform(target, out Transform targetTransform))
        {
            return false;
        }

        Vector3 offset = Vector3.ProjectOnPlane(targetTransform.position - origin.position, Vector3.up);
        return offset.sqrMagnitude <= range * range;
    }

    public static bool IsFacingTarget(Transform origin, IDamageable target)
    {
        if (!TryGetTargetTransform(target, out Transform targetTransform))
        {
            return false;
        }

        Vector3 offset = Vector3.ProjectOnPlane(targetTransform.position - origin.position, Vector3.up);
        return offset.sqrMagnitude <= 0.0001f || Vector3.Dot(origin.forward, offset.normalized) >= FaceDotThreshold;
    }

    public static bool IsHostile(IDamageable selfDamageable, IDamageable target)
    {
        if (target == null)
        {
            return false;
        }
        if (ReferenceEquals(selfDamageable, target))
        {
            return false;
        }

        return selfDamageable == null || target.Team != selfDamageable.Team;
    }

    private static bool TryGetTargetTransform(IDamageable target, out Transform targetTransform)
    {
        if (target is Component targetComponent)
        {
            targetTransform = targetComponent.transform;
            return true;
        }

        targetTransform = null;
        return false;
    }
}
