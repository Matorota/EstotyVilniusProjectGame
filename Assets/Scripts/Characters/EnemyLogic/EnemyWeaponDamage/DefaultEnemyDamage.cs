using UnityEngine;

public class DefaultEnemyDamage : MonoBehaviour, IDamageSource
{
    [SerializeField] float damageAmount = 10f;

    public float DamageAmount => damageAmount;

    void OnValidate()
    {
        damageAmount = Mathf.Max(0f, damageAmount);
    }
}
