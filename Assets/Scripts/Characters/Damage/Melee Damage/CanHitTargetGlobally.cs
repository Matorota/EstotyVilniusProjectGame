public partial class DefaultEnemyDamage
{
    bool CanHitTargetGlobally(int targetId, float currentTime)
    {
        if (!nextGlobalHitTimeByTarget.TryGetValue(targetId, out float nextGlobalHitTime))
        {
            return true;
        }

        return currentTime >= nextGlobalHitTime;
    }
}
