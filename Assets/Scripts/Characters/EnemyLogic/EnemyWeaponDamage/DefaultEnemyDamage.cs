using UnityEngine;
using System.Collections.Generic;

public class DefaultEnemyDamage : MonoBehaviour, IDamageSource
{
    [Header("Damage")]
    [SerializeField] float damageAmount = 10f;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float hitCooldownSeconds = 0.4f;

    readonly Dictionary<int, float> nextHitTimeByTarget = new Dictionary<int, float>();

    public float DamageAmount => damageAmount;

    void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryDamage(hit.collider);
    }

    void OnValidate()
    {
        damageAmount = Mathf.Max(0f, damageAmount);
        hitCooldownSeconds = Mathf.Max(0f, hitCooldownSeconds);
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
        if (nextHitTimeByTarget.TryGetValue(targetId, out float nextHitTime) && Time.time < nextHitTime)
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
}
