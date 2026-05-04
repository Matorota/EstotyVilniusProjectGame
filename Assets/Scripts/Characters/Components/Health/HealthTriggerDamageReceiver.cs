using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthTriggerDamageReceiver : MonoBehaviour
{
    [Header("Trigger Damage")]
    [SerializeField] float defaultTriggerDamage = 10f;

    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        defaultTriggerDamage = Mathf.Max(0f, defaultTriggerDamage);
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
        EnemyMovement enemyMovement =
            other.GetComponent<EnemyMovement>() ??
            other.GetComponentInParent<EnemyMovement>() ??
            other.GetComponentInChildren<EnemyMovement>();

        if (enemyMovement == null)
        {
            return;
        }

        // DefaultEnemyDamage already applies timed contact damage on the enemy side.
        // Skipping it here prevents extra burst damage when moving into enemies.
        DefaultEnemyDamage enemyDamage =
            other.GetComponent<DefaultEnemyDamage>() ??
            other.GetComponentInParent<DefaultEnemyDamage>() ??
            other.GetComponentInChildren<DefaultEnemyDamage>();

        if (enemyDamage != null)
        {
            return;
        }

        health.TakeDamage(defaultTriggerDamage);
    }
}
