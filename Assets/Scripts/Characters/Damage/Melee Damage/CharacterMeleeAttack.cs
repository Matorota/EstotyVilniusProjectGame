using Characters.Health;
using Combat;
using UnityEngine;

[RequireComponent(typeof(CharacterAttackAnimation))]
[RequireComponent(typeof(Combat.Combat))]
public class CharacterMeleeAttack : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 1.25f;
    [SerializeField] private float rangePadding = 0.25f;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private LayerMask targetLayers = ~0;

    [SerializeField] private float hitDelay = 0.45f;
    [SerializeField] private float attackDuration = 0.8f;

    private CharacterAttackAnimation attackAnimation;
    private ICombat combat;
    private CharacterDefense selfDefense;

    private float nextAttackTime;
    private float hitTime;
    private float attackEndTime;

    private bool isAttacking;
    private bool hasHitThisAttack;

    public Transform CurrentTargetTransform => combat?.TargetTransform;
    public float Range => range;

    private void Awake()
    {
        attackAnimation = GetComponent<CharacterAttackAnimation>();
        combat = GetComponent<ICombat>();
        selfDefense = GetComponent<CharacterDefense>();
        if (targetLayers.value == 0)
        {
            targetLayers = ~0;
        }
        range = Mathf.Max(0f, range);
        rangePadding = Mathf.Max(0f, rangePadding);
        cooldown = Mathf.Max(0f, cooldown);
        hitDelay = Mathf.Max(0f, hitDelay);
        attackDuration = Mathf.Max(hitDelay, attackDuration);
    }

    private void FixedUpdate()
    {
        if (combat == null || combat.Self == null)
        {
            return;
        }

        IDamageable self = combat.Self;
        IDamageable target = combat.Target;
        float effectiveRange = range + rangePadding;
        if (ReferenceEquals(target, self))
        {
            combat.SetTarget(null);
            target = null;
        }
        if (!FindTargetables.IsTargetValid(transform, self, target, effectiveRange))
        {
            target = FindTargetables.FindTarget(transform, self, effectiveRange, targetLayers);
            if (target == null && targetLayers.value != ~0)
            {
                target = FindTargetables.FindTarget(transform, self, effectiveRange, ~0);
            }
            combat.SetTarget(target);
        }

        if (isAttacking)
        {
            if (!hasHitThisAttack && Time.time >= hitTime)
            {
                TryApplyDamage();
            }

            if (Time.time >= attackEndTime)
            {
                isAttacking = false;
                hasHitThisAttack = false;
            }

            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        if (target == null || (selfDefense != null && selfDefense.IsDefending))
        {
            return;
        }

        if (!FindTargetables.IsFacingTarget(transform, target))
        {
            return;
        }

        StartAttack();
    }

    private void StartAttack()
    {
        bool animationStarted = attackAnimation.TryPlayAttack();

        if (!animationStarted)
        {
            return;
        }

        isAttacking = true;
        hasHitThisAttack = false;

        hitTime = Time.time + hitDelay;
        attackEndTime = Time.time + attackDuration;
        nextAttackTime = Time.time + cooldown;
    }

    private void TryApplyDamage()
    {
        hasHitThisAttack = true;
        if (combat == null || combat.Self == null)
        {
            return;
        }

        IDamageable self = combat.Self;
        IDamageable target = combat.Target;
        float effectiveRange = range + rangePadding;
        if (target == null)
        {
            return;
        }

        CharacterDefense targetDefense = (target as Component)?.GetComponent<CharacterDefense>();
        if ((targetDefense != null && targetDefense.IsDefending) || (selfDefense != null && selfDefense.IsDefending))
        {
            return;
        }

        if (!FindTargetables.IsTargetValid(transform, self, target, effectiveRange))
        {
            return;
        }

        if (!FindTargetables.IsFacingTarget(transform, target))
        {
            return;
        }

        target.TakeDamage(damage);

        if (target.CurrentHealth <= 0f)
        {
            combat.SetTarget(null);
        }
    }
}
