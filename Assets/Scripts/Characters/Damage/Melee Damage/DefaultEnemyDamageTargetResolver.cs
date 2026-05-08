using Characters.Health;
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
        attackerIsEnemy = owner.TryGetComponent(out EnemyMovement _);
        attackerIsCharacter = owner.TryGetComponent(out CharacterMotor _);
    }

    public bool TryCreateTargetContext(Collider other, float currentTime, out DefaultEnemyDamageTargetContext context)
    {
        context = default;
        if (!IsTarget(other) || !TryGetTargetDamageable(other, out IDamageable targetDamageable))
        {
            return false;
        }

        context.TargetId = other.gameObject.GetInstanceID();
        context.TargetTransform = other.transform;
        context.TargetDamageable = targetDamageable;
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
        if (other.TryGetComponent(out enemyMovement))
        {
            return true;
        }

        if (other.GetComponentInParent<EnemyMovement>() is EnemyMovement parentEnemyMovement)
        {
            enemyMovement = parentEnemyMovement;
            return true;
        }

        if (other.GetComponentInChildren<EnemyMovement>() is EnemyMovement childEnemyMovement)
        {
            enemyMovement = childEnemyMovement;
            return true;
        }

        return false;
    }

    private bool TryGetCharacterMotor(Collider other, out CharacterMotor characterMotor)
    {
        if (other.TryGetComponent(out characterMotor))
        {
            return true;
        }

        if (other.GetComponentInParent<CharacterMotor>() is CharacterMotor parentCharacterMotor)
        {
            characterMotor = parentCharacterMotor;
            return true;
        }

        if (other.GetComponentInChildren<CharacterMotor>() is CharacterMotor childCharacterMotor)
        {
            characterMotor = childCharacterMotor;
            return true;
        }

        return false;
    }

    private bool TryGetTargetDamageable(Collider other, out IDamageable targetDamageable)
    {
        MonoBehaviour[] behaviours = other.gameObject.GetComponents<MonoBehaviour>();
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is IDamageable damageable)
            {
                targetDamageable = damageable;
                return true;
            }
        }

        targetDamageable = default!;
        return false;
    }
}
