using UnityEngine;

public class DefaultEnemyDamageTargetResolver
{
    private  float enemyDamageMultiplier;
    private  float characterDamageMultiplier;
    private  bool attackerIsEnemy;
    private  bool attackerIsCharacter;

    public DefaultEnemyDamageTargetResolver(MonoBehaviour owner, float enemyDamageMultiplier, float characterDamageMultiplier)
    {
        this.enemyDamageMultiplier = enemyDamageMultiplier;
        this.characterDamageMultiplier = characterDamageMultiplier;
        attackerIsEnemy = owner.GetComponent<EnemyMovement>() != null;
        attackerIsCharacter = owner.GetComponent<CharacterMotor>() != null;
    }

    public bool TryCreateTargetContext(Collider other, float currentTime, out DefaultEnemyDamageTargetContext context)
    {
        context = default;
        if (other == null || !IsTarget(other) || !TryGetTargetHealth(other, out Health targetHealth))
        {
            return false;
        }

        context.TargetId = targetHealth.GetInstanceID();
        context.TargetHealth = targetHealth;
        context.CurrentTime = currentTime;
        context.TargetDamageMultiplier = ResolveTargetDamageMultiplier(other);
        return true;
    }

    private bool IsTarget(Collider other)
    {
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

    private bool TryGetTargetHealth(Collider other, out Health targetHealth)
    {
        targetHealth =
            other.GetComponent<Health>() ??
            other.GetComponentInParent<Health>() ??
            other.GetComponentInChildren<Health>();

        return targetHealth != null;
    }
}
