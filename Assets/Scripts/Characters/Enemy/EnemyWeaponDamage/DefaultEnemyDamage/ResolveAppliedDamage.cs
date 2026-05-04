public partial class DefaultEnemyDamage
{
    float ResolveAppliedDamage(float targetTowardSpeed)
    {
        if (targetTowardSpeed < movingTowardSpeedThreshold)
        {
            return damageAmount;
        }

        return damageAmount * movingTowardDamageMultiplier;
    }
}
