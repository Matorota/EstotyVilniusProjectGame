using Characters.Health;
using UnityEngine;

[RequireComponent(typeof(CharacterAttackAnimation))]
[RequireComponent(typeof(FindTargetables))]
public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float playerDamage = 10f;
    [SerializeField] private float enemyDamage = 4f;
    [SerializeField] private float range = 1.25f;
    [SerializeField] private float cooldown = 1.5f;

    private CharacterAttackAnimation attackAnimation;
    private FindTargetables targetables;
    private IDamageable selfDamageable;
    private IDamageable currentTarget;
    private float nextAttackTime;

    public Transform CurrentTargetTransform => currentTarget != null ? ((Component)currentTarget).transform : null;
    public float Range => range;

    private void Awake()
    {
        attackAnimation = GetComponent<CharacterAttackAnimation>();
        targetables = GetComponent<FindTargetables>() ?? gameObject.AddComponent<FindTargetables>();
        selfDamageable = GetComponent<IDamageable>();
        range = Mathf.Max(0f, range);
        cooldown = Mathf.Max(0f, cooldown);
    }

    private void FixedUpdate()
    {
        if (!targetables.IsTargetValid(transform, selfDamageable, currentTarget, range))
        {
            currentTarget = targetables.FindTarget(transform, selfDamageable, range);
        }

        if (currentTarget == null || Time.time < nextAttackTime || !targetables.IsFacingTarget(transform, currentTarget))
        {
            return;
        }

        if (selfDamageable != null && selfDamageable.Team == Characters.Team.TeamId.Player && selfDamageable.IsDefending)
        {
            return;
        }

        attackAnimation.TryPlayAttack();
        currentTarget.TakeDamage(selfDamageable != null && selfDamageable.Team == Characters.Team.TeamId.Enemy ? enemyDamage : playerDamage);
        nextAttackTime = Time.time + cooldown;

        if (currentTarget.CurrentHealth <= 0f)
        {
            currentTarget = null;
        }
    }
}
