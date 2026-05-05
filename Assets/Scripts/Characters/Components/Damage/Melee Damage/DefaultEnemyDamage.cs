using UnityEngine;
using System.Collections.Generic;

public partial class DefaultEnemyDamage : MonoBehaviour
{
    const float MinimumDamageAmount = 10f;
    const float MaximumDamageAmount = 20f;
    const float MinimumHitCooldown = 1f;
    const float MinimumInitialContactDelay = 0.2f;
    const float MinimumGlobalHitCooldown = 0.4f;

    [Header("Damage")]
    [SerializeField] float damageAmount = 20f;
    [SerializeField] float hitCooldownSeconds = 1.4f;
    [SerializeField] float initialContactDelaySeconds = 0.7f;
    [SerializeField] float globalHitCooldownSeconds = 0.8f;
    [SerializeField] float movingTowardDamageMultiplier = 1.2f;
    [SerializeField] float movingTowardSpeedThreshold = 0.45f;
    [SerializeField] float enemyDamageMultiplier = 1.5f;
    [SerializeField] float characterDamageMultiplier = 1f;

    [Header("Contact")]
    [SerializeField] float damageRadius = 1.25f;

    readonly Dictionary<int, float> nextHitTimeByTarget = new Dictionary<int, float>();
    static readonly Dictionary<int, float> nextGlobalHitTimeByTarget = new Dictionary<int, float>();
    readonly Dictionary<int, Vector3> lastTargetPositionById = new Dictionary<int, Vector3>();
    readonly Dictionary<int, float> lastTargetSampleTimeById = new Dictionary<int, float>();

    struct TargetContactContext
    {
        public int TargetId;
        public Health TargetHealth;
        public float CurrentTime;
        public float TowardSpeed;
        public float TargetDamageMultiplier;
    }

    private void Awake()
    {
        NormalizeDamageValues();
    }

    private void OnValidate()
    {
        NormalizeDamageValues();
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

    bool IsTarget(Collider other)
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

    bool TryCreateTargetContext(Collider other, out TargetContactContext context)
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
        context.TowardSpeed = SampleTargetTowardSpeed(targetId, targetHealth.transform.position);
        context.TargetDamageMultiplier = ResolveTargetDamageMultiplier(other);
        return true;
    }

    private void ApplyDamage(TargetContactContext context)
    {
        float appliedDamage = ResolveAppliedDamage(context.TowardSpeed) * context.TargetDamageMultiplier;
        context.TargetHealth.TakeDamage(appliedDamage);
        nextHitTimeByTarget[context.TargetId] = context.CurrentTime + hitCooldownSeconds;
        nextGlobalHitTimeByTarget[context.TargetId] = context.CurrentTime + globalHitCooldownSeconds;
    }

    private void NormalizeDamageValues()
    {
        damageAmount = Mathf.Clamp(damageAmount, MinimumDamageAmount, MaximumDamageAmount);
        hitCooldownSeconds = Mathf.Max(MinimumHitCooldown, hitCooldownSeconds);
        initialContactDelaySeconds = Mathf.Max(MinimumInitialContactDelay, initialContactDelaySeconds);
        globalHitCooldownSeconds = Mathf.Max(MinimumGlobalHitCooldown, globalHitCooldownSeconds);
        movingTowardDamageMultiplier = Mathf.Clamp(movingTowardDamageMultiplier, 1f, 1.5f);
        movingTowardSpeedThreshold = Mathf.Max(0.01f, movingTowardSpeedThreshold);
        damageRadius = Mathf.Max(0.1f, damageRadius);
        enemyDamageMultiplier = Mathf.Max(0f, enemyDamageMultiplier);
        characterDamageMultiplier = Mathf.Max(0f, characterDamageMultiplier);
    }

    float ResolveTargetDamageMultiplier(Collider other)
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

    bool TryGetEnemyMovement(Collider other, out EnemyMovement enemyMovement)
    {
        enemyMovement =
            other.GetComponent<EnemyMovement>() ??
            other.GetComponentInParent<EnemyMovement>() ??
            other.GetComponentInChildren<EnemyMovement>();

        return enemyMovement != null;
    }

    bool TryGetCharacterMotor(Collider other, out CharacterMotor characterMotor)
    {
        characterMotor =
            other.GetComponent<CharacterMotor>() ??
            other.GetComponentInParent<CharacterMotor>() ??
            other.GetComponentInChildren<CharacterMotor>();

        return characterMotor != null;
    }

    float SampleTargetTowardSpeed(int targetId, Vector3 targetPosition)
    {
        float currentTime = Time.time;
        float towardSpeed = 0f;

        if (lastTargetPositionById.TryGetValue(targetId, out Vector3 previousPosition) &&
            lastTargetSampleTimeById.TryGetValue(targetId, out float previousTime))
        {
            float deltaTime = currentTime - previousTime;
            if (deltaTime > 0.0001f)
            {
                Vector3 targetVelocity = (targetPosition - previousPosition) / deltaTime;
                Vector3 towardEnemy = transform.position - targetPosition;
                towardEnemy.y = 0f;
                if (towardEnemy.sqrMagnitude > 0.0001f)
                {
                    towardSpeed = Vector3.Dot(targetVelocity, towardEnemy.normalized);
                }
            }
        }

        lastTargetPositionById[targetId] = targetPosition;
        lastTargetSampleTimeById[targetId] = currentTime;
        return Mathf.Max(0f, towardSpeed);
    }
}
