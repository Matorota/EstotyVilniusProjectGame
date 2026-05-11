using UnityEngine;

public class Combat : MonoBehaviour, ICombat
{
    [SerializeField] private IDamageable target;
    [SerializeField] private CharacterDefense targetDefense;
    private IDamageable self;
    private CharacterDefense selfDefense;

    public bool HasValidSelf => Self != null;
    public IDamageable Self => self;
    public IDamageable Target
    {
        get
        {
            CleanupDestroyedReferences();
            return target;
        }
    }
    public Transform TargetTransform
    {
        get
        {
            CleanupDestroyedReferences();
            return GetTargetComponent()?.transform;
        }
    }
    public CharacterDefense TargetDefense
    {
        get
        {
            CleanupDestroyedReferences();
            targetDefense ??= GetTargetComponent()?.GetComponent<CharacterDefense>();
            return targetDefense;
        }
    }
    public bool IsSelfDefending => selfDefense != null && selfDefense.IsDefending;
    public bool IsTargetDefending => TargetDefense != null && TargetDefense.IsDefending;

    private void Awake()
    {
        self = GetComponent<IDamageable>() ?? GetComponentInParent<IDamageable>();
        selfDefense = GetComponent<CharacterDefense>();
        target ??= targetDefense != null ? targetDefense.GetComponent<IDamageable>() : null;
        targetDefense ??= GetTargetComponent()?.GetComponent<CharacterDefense>();

        if (ReferenceEquals(target, self)) ClearTarget();
        CleanupDestroyedReferences();
    }

    public void SetTarget(IDamageable newTarget)
    {
        if (newTarget == null || ReferenceEquals(newTarget, self))
        {
            ClearTarget();
            return;
        }

        target = newTarget;
        targetDefense = GetTargetComponent()?.GetComponent<CharacterDefense>();
        CleanupDestroyedReferences();
    }

    public void ClearTarget()
    {
        target = null;
        targetDefense = null;
    }

    private void CleanupDestroyedReferences()
    {
        if (GetTargetComponent() == null) target = null;
        if (IsDestroyed(targetDefense)) targetDefense = null;
        if (self is Component selfComponent && selfComponent == null) self = null;
        if (IsDestroyed(selfDefense)) selfDefense = null;
    }

    private Component GetTargetComponent() => target as Component;
    private static bool IsDestroyed(Object unityObject) => !ReferenceEquals(unityObject, null) && unityObject == null;
}
