using UnityEngine;

public class Combat : MonoBehaviour
{
    private IDamageable target;
    private IDamageable self;

    public bool HasValidSelf => self != null;
    public IDamageable Self => self;
    public IDamageable Target => target;
    public Transform TargetTransform => (target as Component)?.transform;

    private void Awake()
    {
        self = GetComponent<IDamageable>() ?? GetComponentInParent<IDamageable>();
        if (ReferenceEquals(target, self)) ClearTarget();
    }

    public void SetTarget(IDamageable newTarget)
    {
        if (newTarget == null || ReferenceEquals(newTarget, self))
        {
            ClearTarget();
            return;
        }

        target = newTarget;
    }

    public void ClearTarget()
    {
        target = null;
    }
}
