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

    [SerializeField] private float hitDelay = 0.45f;
    [SerializeField] private float attackDuration = 0.8f;

    private CharacterAttackAnimation attackAnimation;
    private ICombat combat;

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

        if (!FindTargetables.IsFacingTarget(transform, target)) // leaving for now
        {
            return;
        }

        StartAttack();
    }
    private bool CanRunCombat() // added after refactoring kept on attacking friendlies (enemy to enemy ) 
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

        if (!FindTargetables.IsTargetValid(transform, self, target, EffectiveRange))
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
            combat.ClearTarget();
        }
    }

    private void ResolveTarget()
    {
        IDamageable self = combat.Self;
        IDamageable target = combat.Target;
        if (FindTargetables.IsTargetValid(transform, self, target, EffectiveRange))
        {
            return;
        }

        target = FindTargetables.FindTarget(transform, self, EffectiveRange);

        if (target == null)
        {
            combat.ClearTarget();
            return;
        }

        combat.SetTarget(target);
    }


}
