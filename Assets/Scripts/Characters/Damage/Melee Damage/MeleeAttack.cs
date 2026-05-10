using Characters.Health;
using Characters.Team;
using UnityEngine;

[RequireComponent(typeof(CharacterAttackAnimation))]
[RequireComponent(typeof(FindTargetables))]
public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float playerDamage = 10f;
    [SerializeField] private float enemyDamage = 4f;
    [SerializeField] private float range = 1.25f;
    [SerializeField] private float cooldown = 1.5f;

    [Header("Code-Based Hit Timing")]
    [SerializeField] private float hitDelay = 0.45f;
    [SerializeField] private float attackDuration = 0.8f;

    private CharacterAttackAnimation attackAnimation;
    private FindTargetables targetFinder;
    private IDamageable self;
    private IDamageable target;

    private float nextAttackTime;
    private float hitTime;
    private float attackEndTime;

    private bool isAttacking;
    private bool hasHitThisAttack;

    public Transform CurrentTargetTransform => target != null ? ((Component)target).transform : null;
    public float Range => range;

    private bool IsEnemy => self != null && self.Team == TeamId.Enemy;
    private float Damage => IsEnemy ? enemyDamage : playerDamage;

    private void Awake()
    {
        attackAnimation = GetComponent<CharacterAttackAnimation>();
        targetFinder = GetComponent<FindTargetables>();
        self = GetComponent<IDamageable>();

        range = Mathf.Max(0f, range);
        cooldown = Mathf.Max(0f, cooldown);
        hitDelay = Mathf.Max(0f, hitDelay);
        attackDuration = Mathf.Max(hitDelay, attackDuration);
    }

    private void FixedUpdate()
    {
        if (!targetFinder.IsTargetValid(transform, self, target, range))
        {
            target = targetFinder.FindTarget(transform, self, range);
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

        if (target == null || Time.time < nextAttackTime)
        {
            return;
        }

        if (self != null && self.IsDefending)
        {
            return;
        }

        if (!targetFinder.IsFacingTarget(transform, target))
        {
            return;
        }

        StartAttack();
    }

    private void StartAttack()
    {
        bool animationStarted = IsEnemy
            ? attackAnimation.ForcePlayAttack()
            : attackAnimation.TryPlayAttack();

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

        if (target == null || target.IsDefending)
        {
            return;
        }

        if (self != null && self.IsDefending)
        {
            return;
        }

        if (!targetFinder.IsTargetValid(transform, self, target, range))
        {
            return;
        }

        if (!targetFinder.IsFacingTarget(transform, target))
        {
            return;
        }

        target.TakeDamage(Damage);

        if (target.CurrentHealth <= 0f)
        {
            target = null;
        }
    }
}