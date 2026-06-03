using UnityEngine;

[RequireComponent(typeof(CharacterAttackAnimation))]
[RequireComponent(typeof(Combat))]
public class CharacterMeleeAttack : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 1.25f;
    [SerializeField] private float rangePadding = 0.25f;
    [SerializeField] private float cooldown = 1.5f;

    [SerializeField] private float hitDelay = 0.45f;
    [SerializeField] private float attackDuration = 0.8f;

    [SerializeField] private LayerMask targetMask = ~0;

    private CharacterAttackAnimation attackAnimation;
    private Combat combat;
    private FindTargetables targetables;
    private PlayerStats stats;
    private Health selfHealth;

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
        combat = GetComponent<Combat>();
        targetables = new FindTargetables();
        stats = GetComponent<PlayerStats>();
        selfHealth = GetComponent<Health>();
        range = Mathf.Max(0f, range);
    }

    private void FixedUpdate()
    {
        if (!CanRunCombat())
        {
            StopAttack();
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
        if (target == null || IsSelfDefending)
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
        return combat != null && combat.HasValidSelf && (selfHealth == null || selfHealth.CurrentHealth > 0f);
    }

    private void StopAttack()
    {
        isAttacking = false;
        hasHitThisAttack = false;

        if (combat != null)
            combat.ClearTarget();
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
        if (target == null || IsSelfDefending || IsTargetDefending(target))
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

    private bool IsSelfDefending => GetComponent<CharacterDefense>()?.IsDefending ?? false;

    private bool IsTargetDefending(IDamageable target)
    {
        if (target is Component comp)
            return comp.GetComponent<CharacterDefense>()?.IsDefending ?? false;
        return false;
    }

    private void ResolveTarget()
    {
        IDamageable self = combat.Self;
        IDamageable target = combat.Target;
        if (targetables.IsTargetValid(transform, self, target, EffectiveRange))
        {
            return;
        }

        target = targetables.FindTarget(transform, self, EffectiveRange, targetMask);

        if (target == null)
        {
            combat.ClearTarget();
            return;
        }

        combat.SetTarget(target);
    }
}
