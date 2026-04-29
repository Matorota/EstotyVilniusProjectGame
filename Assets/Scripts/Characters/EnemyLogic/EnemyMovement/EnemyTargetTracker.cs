using UnityEngine;

public class EnemyTargetTracker : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] float refreshInterval = 0.3f;

    Transform currentTarget;
    float nextSearchTime;

    public Transform CurrentTarget => currentTarget;

    public Transform ResolveTarget()
    {
        if (currentTarget != null)
        {
            return currentTarget;
        }

        if (Time.time < nextSearchTime)
        {
            return null;
        }

        nextSearchTime = Time.time + refreshInterval;
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        currentTarget = playerObject != null ? playerObject.transform : null;
        return currentTarget;
    }

    void OnValidate()
    {
        refreshInterval = Mathf.Max(0.05f, refreshInterval);
    }
}
