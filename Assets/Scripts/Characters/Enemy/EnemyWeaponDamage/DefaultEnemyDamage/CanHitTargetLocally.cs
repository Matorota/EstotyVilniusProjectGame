public partial class DefaultEnemyDamage
{
    bool CanHitTargetLocally(int targetId, float currentTime)
    {
        if (!nextHitTimeByTarget.TryGetValue(targetId, out float nextHitTime))
        {
            nextHitTimeByTarget[targetId] = currentTime + initialContactDelaySeconds;
            return false;
        }

        return currentTime >= nextHitTime;
    }
}
