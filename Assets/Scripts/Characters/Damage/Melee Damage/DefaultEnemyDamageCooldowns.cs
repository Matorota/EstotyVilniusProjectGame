using System.Collections.Generic;

public class DefaultEnemyDamageCooldowns
{
    private readonly Dictionary<int, float> nextHitTimeByTarget = new Dictionary<int, float>();
    private static readonly Dictionary<int, float> nextGlobalHitTimeByTarget = new Dictionary<int, float>();

    public bool CanHitTargetLocally(int targetId, float currentTime, float initialContactDelaySeconds)
    {
        if (!nextHitTimeByTarget.TryGetValue(targetId, out float nextHitTime))
        {
            nextHitTimeByTarget[targetId] = currentTime + initialContactDelaySeconds;
            return false;
        }

        return currentTime >= nextHitTime;
    }

    public bool CanHitTargetGlobally(int targetId, float currentTime)
    {
        if (!nextGlobalHitTimeByTarget.TryGetValue(targetId, out float nextGlobalHitTime))
        {
            return true;
        }

        return currentTime >= nextGlobalHitTime;
    }

    public void RegisterHit(int targetId, float currentTime, float hitCooldownSeconds, float globalHitCooldownSeconds)
    {
        nextHitTimeByTarget[targetId] = currentTime + hitCooldownSeconds;
        nextGlobalHitTimeByTarget[targetId] = currentTime + globalHitCooldownSeconds;
    }
}
