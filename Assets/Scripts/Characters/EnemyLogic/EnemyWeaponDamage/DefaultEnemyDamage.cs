using UnityEngine;
using System.Collections.Generic;

public class DefaultEnemyDamage : MonoBehaviour, IDamageSource
{
    const float MaximumDamageAmount = 4f;
    const float MinimumHitCooldown = 1f;
    const float MinimumInitialContactDelay = 0.2f;

    [Header("Damage")]
    [SerializeField] float damageAmount = 3f;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float hitCooldownSeconds = 1.2f;
    [SerializeField] float initialContactDelaySeconds = 0.6f;

    [Header("Contact")]
    [SerializeField] float damageRadius = 1.25f;

    readonly Dictionary<int, float> nextHitTimeByTarget = new Dictionary<int, float>();

    public float DamageAmount => damageAmount;

    void Awake()
    {
        NormalizeDamageValues();
    }

    void OnValidate()
    {
        NormalizeDamageValues();
    }

    void FixedUpdate()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, damageRadius, Physics.AllLayers, QueryTriggerInteraction.Collide);
        for (int i = 0; i < nearbyColliders.Length; i++)
        {
            TryDamage(nearbyColliders[i]);
        }
    }

    void TryDamage(Collider other)
    {
        if (other == null || !IsTarget(other))
        {
            return;
        }

        Health targetHealth =
            other.GetComponent<Health>() ??
            other.GetComponentInParent<Health>() ??
            other.GetComponentInChildren<Health>();
        if (targetHealth == null)
        {
            return;
        }

        int targetId = targetHealth.GetInstanceID();
        if (!nextHitTimeByTarget.TryGetValue(targetId, out float nextHitTime))
        {
            nextHitTimeByTarget[targetId] = Time.time + initialContactDelaySeconds;
            return;
        }

        if (Time.time < nextHitTime)
        {
            return;
        }

        targetHealth.TakeDamage(damageAmount);
        nextHitTimeByTarget[targetId] = Time.time + hitCooldownSeconds;
    }

    bool IsTarget(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            return true;
        }

        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(targetTag))
        {
            return true;
        }

        return other.transform.root.CompareTag(targetTag);
    }

    void NormalizeDamageValues()
    {
        damageAmount = Mathf.Clamp(damageAmount, 0f, MaximumDamageAmount);
        hitCooldownSeconds = Mathf.Max(MinimumHitCooldown, hitCooldownSeconds);
        initialContactDelaySeconds = Mathf.Max(MinimumInitialContactDelay, initialContactDelaySeconds);
        damageRadius = Mathf.Max(0.1f, damageRadius);
    }
}
