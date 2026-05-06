using UnityEngine;

public class DefaultEnemyDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float hitCooldownSeconds = 1.4f;
    [SerializeField] private float initialContactDelaySeconds = 0.7f;
    [SerializeField] private float globalHitCooldownSeconds = 0.8f;
    [SerializeField] private float enemyDamageMultiplier = 1.5f;
    [SerializeField] private float characterDamageMultiplier = 1f;

    [SerializeField] private float damageRadius = 1.25f;

    private DefaultEnemyDamageTargetResolver targetResolver;
    private DefaultEnemyDamageCooldowns cooldowns;

    private void Awake()
    {
        targetResolver = new DefaultEnemyDamageTargetResolver(this, enemyDamageMultiplier, characterDamageMultiplier);
        cooldowns = new DefaultEnemyDamageCooldowns();
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

        if (!cooldowns.CanHitTargetLocally(context.TargetId, context.CurrentTime, initialContactDelaySeconds))
        {
            return;
        }

        if (!cooldowns.CanHitTargetGlobally(context.TargetId, context.CurrentTime))
        {
            return;
        }

        float appliedDamage = damageAmount * context.TargetDamageMultiplier;
        context.TargetHealth.TakeDamage(appliedDamage);
        cooldowns.RegisterHit(context.TargetId, context.CurrentTime, hitCooldownSeconds, globalHitCooldownSeconds);
    }
}
