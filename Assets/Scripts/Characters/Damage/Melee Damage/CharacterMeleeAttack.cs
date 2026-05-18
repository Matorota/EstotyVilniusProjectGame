using UnityEngine;

[RequireComponent(typeof(CharacterAttackAnimation))]
[RequireComponent(typeof(Combat))]
[RequireComponent(typeof(FindTargetables))]
public class CharacterMeleeAttack : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 1.25f;
    [SerializeField] private float rangePadding = 0.25f;
    [SerializeField] private float cooldown = 1.5f;

    [SerializeField] private float hitDelay = 0.45f;
    [SerializeField] private float attackDuration = 0.8f;

    private CharacterAttackAnimation attackAnimation;
    private ICombat combat;
    private FindTargetables targetables;
    private PlayerStats stats;

    private float nextAttackTime;
    private float hitTime;
    private float attackEndTime;

    private bool isAttacking;
    private bool hasHitThisAttack;

    public Transform CurrentTargetTransform => combat?.TargetTransform;
    public float Range => range;
    private float EffectiveRange => range + rangePadding;

    private void Awake()
    {
        attackAnimation = GetComponent<CharacterAttackAnimation>();
        combat = GetComponent<ICombat>();
        targetables = GetComponent<FindTargetables>();
        stats = GetComponent<PlayerStats>();
        range = Mathf.Max(0f, range);
    }

    private void FixedUpdate()
    {
        if (!CanRunCombat())
        {
            return;
        }

        ResolveTarget();

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

        IDamageable target = combat.Target;
        if (target == null || combat.IsSelfDefending)
        {
            return;
        }

        if (!targetables.IsFacingTarget(transform, target))
        {
            return;
        }

        StartAttack();
    }

    private bool CanRunCombat()
    {
        return combat != null && combat.HasValidSelf;
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
        if (!CanRunCombat())
        {
            return;
        }

        IDamageable self = combat.Self;
        IDamageable target = combat.Target;
        if (target == null || combat.IsSelfDefending || combat.IsTargetDefending)
        {
            return;
        }

        if (!targetables.IsTargetValid(transform, self, target, EffectiveRange))
        {
            return;
        }

        if (!targetables.IsFacingTarget(transform, target))
        {
            return;
        }

        float bonus = stats != null ? stats.GetDamageBonus() : 0f;
        float finalDamage = damage + bonus;

        target.TakeDamage(finalDamage);

        if (target.CurrentHealth <= 0f)
        {
            combat.ClearTarget();
        }
    }

    private void ResolveTarget()
    {
        IDamageable self = combat.Self;
        IDamageable target = combat.Target;
        if (targetables.IsTargetValid(transform, self, target, EffectiveRange))
        {
            return;
        }

        target = targetables.FindTarget(transform, self, EffectiveRange);

        if (target == null)
        {
            combat.ClearTarget();
            return;
        }

        combat.SetTarget(target);
    }
}
