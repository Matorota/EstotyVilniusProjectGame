using UnityEngine;
using System.Collections.Generic;

public partial class DefaultEnemyDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float hitCooldownSeconds = 1.4f;
    [SerializeField] private float initialContactDelaySeconds = 0.7f;
    [SerializeField] private float globalHitCooldownSeconds = 0.8f;
    [SerializeField] private float enemyDamageMultiplier = 1.5f;
    [SerializeField] private float characterDamageMultiplier = 1f;

    [SerializeField] private float damageRadius = 1.25f;

    private readonly Dictionary<int, float> nextHitTimeByTarget = new Dictionary<int, float>();
    private static readonly Dictionary<int, float> nextGlobalHitTimeByTarget = new Dictionary<int, float>();

    private struct TargetContactContext
    {
        public int TargetId;
        public Health TargetHealth;
        public float CurrentTime;
        public float TargetDamageMultiplier;
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
        if (!TryCreateTargetContext(other, out TargetContactContext context))
        {
            return;
        }

        if (!CanHitTargetLocally(context.TargetId, context.CurrentTime))
        {
            return;
        }

        if (!CanHitTargetGlobally(context.TargetId, context.CurrentTime))
        {
            return;
        }

        ApplyDamage(context);
    }

    private bool IsTarget(Collider other)
    {
        if (other == null)
        {
            return false;
        }

        bool attackerIsEnemy = GetComponent<EnemyMovement>() != null;
        bool attackerIsCharacter = GetComponent<CharacterMotor>() != null;

        if (attackerIsEnemy)
        {
            return TryGetCharacterMotor(other, out _);
        }

        if (attackerIsCharacter)
        {
            return TryGetEnemyMovement(other, out _);
        }

        return TryGetEnemyMovement(other, out _) || TryGetCharacterMotor(other, out _);
    }

    private bool TryCreateTargetContext(Collider other, out TargetContactContext context)
    {
        context = default;
        if (other == null || !IsTarget(other) || !TryGetTargetHealth(other, out Health targetHealth))
        {
            return false;
        }

        int targetId = targetHealth.GetInstanceID();
        context.TargetId = targetId;
        context.TargetHealth = targetHealth;
        context.CurrentTime = Time.time;
        context.TargetDamageMultiplier = ResolveTargetDamageMultiplier(other);
        return true;
    }

    private void ApplyDamage(TargetContactContext context)
    {
        float appliedDamage = damageAmount * context.TargetDamageMultiplier;
        context.TargetHealth.TakeDamage(appliedDamage);
        nextHitTimeByTarget[context.TargetId] = context.CurrentTime + hitCooldownSeconds;
        nextGlobalHitTimeByTarget[context.TargetId] = context.CurrentTime + globalHitCooldownSeconds;
    }

    private float ResolveTargetDamageMultiplier(Collider other)
    {
        if (TryGetEnemyMovement(other, out _))
        {
            return enemyDamageMultiplier;
        }

        if (TryGetCharacterMotor(other, out _))
        {
            return characterDamageMultiplier;
        }

        return 1f;
    }

    private bool TryGetEnemyMovement(Collider other, out EnemyMovement enemyMovement)
    {
        enemyMovement =
            other.GetComponent<EnemyMovement>() ??
            other.GetComponentInParent<EnemyMovement>() ??
            other.GetComponentInChildren<EnemyMovement>();

        return enemyMovement != null;
    }

    private bool TryGetCharacterMotor(Collider other, out CharacterMotor characterMotor)
    {
        characterMotor =
            other.GetComponent<CharacterMotor>() ??
            other.GetComponentInParent<CharacterMotor>() ??
            other.GetComponentInChildren<CharacterMotor>();

        return characterMotor != null;
    }
}
