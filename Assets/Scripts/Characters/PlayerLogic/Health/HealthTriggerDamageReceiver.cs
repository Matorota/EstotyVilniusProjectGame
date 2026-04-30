using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthTriggerDamageReceiver : MonoBehaviour
{
    [Header("Trigger Damage")]
    [SerializeField] string damagingTag = "Enemy";
    [SerializeField] float defaultTriggerDamage = 10f;

    Health health;

    void Awake()
    {
        CacheDependencies();
    }

    void OnValidate()
    {
        defaultTriggerDamage = Mathf.Max(0f, defaultTriggerDamage);
        CacheDependencies();
    }

    void OnTriggerEnter(Collider other)
    {
        TryApplyDamage(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryApplyDamage(collision.collider);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryApplyDamage(hit.collider);
    }

    void TryApplyDamage(Collider other)
    {
        if (health == null || !IsDamagingTag(other))
        {
            return;
        }

        IDamageSource damageSource =
            other.GetComponent<IDamageSource>() ??
            other.GetComponentInParent<IDamageSource>() ??
            other.GetComponentInChildren<IDamageSource>();

        // DefaultEnemyDamage already applies timed contact damage on the enemy side.
        // Skipping it here prevents extra burst damage when moving into enemies.
        if (damageSource is DefaultEnemyDamage)
        {
            return;
        }

        float damageAmount = damageSource != null ? damageSource.DamageAmount : defaultTriggerDamage;

        health.TakeDamage(damageAmount);
    }

    bool IsDamagingTag(Collider other)
    {
        if (other.CompareTag(damagingTag))
        {
            return true;
        }

        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(damagingTag))
        {
            return true;
        }

        return other.transform.root.CompareTag(damagingTag);
    }

    void CacheDependencies()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }
}
