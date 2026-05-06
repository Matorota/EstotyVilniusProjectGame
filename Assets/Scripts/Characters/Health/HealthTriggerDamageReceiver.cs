using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthTriggerDamageReceiver : MonoBehaviour
{
    [SerializeField] private float defaultTriggerDamage = 10f;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        defaultTriggerDamage = Mathf.Max(0f, defaultTriggerDamage);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryApplyDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryApplyDamage(collision.collider);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryApplyDamage(hit.collider);
    }

    private void TryApplyDamage(Collider other)
    {
        EnemyMovement enemyMovement =
            other.GetComponent<EnemyMovement>() ??
            other.GetComponentInParent<EnemyMovement>() ??
            other.GetComponentInChildren<EnemyMovement>();

        if (enemyMovement == null)
        {
            return;
        }

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
