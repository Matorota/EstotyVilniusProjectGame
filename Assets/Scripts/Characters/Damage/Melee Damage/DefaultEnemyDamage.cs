using UnityEngine;

[RequireComponent(typeof(CharacterAttackAnimation))]
public class DefaultEnemyDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float hitCooldownSeconds = 1.4f;
    [SerializeField] private float initialContactDelaySeconds = 0.7f;
    [SerializeField] private float globalHitCooldownSeconds = 0.8f;
    [SerializeField] private float enemyDamageMultiplier = 1.5f;
    [SerializeField] private float characterDamageMultiplier = 1f;

    [SerializeField] private float damageRadius = 1.25f;
    [SerializeField] private float minForwardDot = 0.5f;
    [SerializeField] private float attackRotationSpeedDegrees = 540f;
    [SerializeField] private bool useAnimationHitWindow;
    [SerializeField] private CharacterAttackAnimation attackAnimation;

    private DefaultEnemyDamageTargetResolver targetResolver;
    private DefaultEnemyDamageCooldowns cooldowns;

    private void Awake()
    {
        targetResolver = new DefaultEnemyDamageTargetResolver(this, enemyDamageMultiplier, characterDamageMultiplier);
        cooldowns = new DefaultEnemyDamageCooldowns();
        attackAnimation = GetComponent<CharacterAttackAnimation>();
        if (minForwardDot < -1f)
        {
            minForwardDot = -1f;
        }
        else if (minForwardDot > 1f)
        {
            minForwardDot = 1f;
        }

        if (attackRotationSpeedDegrees < 0f)
        {
            attackRotationSpeedDegrees = 0f;
        }
    }

    private void FixedUpdate()
    {
        Collider[] contacts = Physics.OverlapSphere(
            transform.position,
            damageRadius,
            Physics.AllLayers,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < contacts.Length; i++)
        {
            TryDamage(contacts[i]);
        }
    }

    private void TryDamage(Collider other)
    {
        if (!targetResolver.TryCreateTargetContext(other, Time.time, out DefaultEnemyDamageTargetContext context))
        {
            return;
        }

        RotateTowardsTarget(context.TargetTransform);
        if (!IsFacingTarget(context.TargetTransform))
        {
            return;
        }

        if (!cooldowns.CanHitTargetLocally(context.TargetId, context.CurrentTime, initialContactDelaySeconds))
        {
            return;
        }

        if (!cooldowns.CanHitTargetGlobally(context.TargetId, context.CurrentTime))
        {
            return;
        }
        if (attackAnimation.IsDefending)
        {
            return;
        }


        attackAnimation.TryPlayAttack();
        if (useAnimationHitWindow && !attackAnimation.IsHitWindowOpen)
        {
            return;
        }

        float appliedDamage = damageAmount * context.TargetDamageMultiplier;
        context.TargetDamageable.TakeDamage(appliedDamage);
        cooldowns.RegisterHit(context.TargetId, context.CurrentTime, hitCooldownSeconds, globalHitCooldownSeconds);
    }

    private void RotateTowardsTarget(Transform targetTransform)
    {
        Vector3 toTarget = targetTransform.position - transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude <= 0.000001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(toTarget);
        float maxStep = attackRotationSpeedDegrees * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxStep);
    }

    private bool IsFacingTarget(Transform targetTransform)
    {
        Vector3 toTarget = targetTransform.position - transform.position;
        toTarget.y = 0f;

        float toTargetMagnitude = toTarget.magnitude;
        if (toTargetMagnitude <= 0.0001f)
        {
            return true;
        }

        toTarget /= toTargetMagnitude;

        Vector3 forward = transform.forward;
        forward.y = 0f;

        float forwardMagnitude = forward.magnitude;
        if (forwardMagnitude <= 0.0001f)
        {
            return false;
        }

        forward /= forwardMagnitude;
        return Vector3.Dot(forward, toTarget) >= minForwardDot;
    }
}
