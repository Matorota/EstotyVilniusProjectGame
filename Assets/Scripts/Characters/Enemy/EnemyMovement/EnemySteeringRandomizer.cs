using UnityEngine;

public class EnemySteeringRandomizer : MonoBehaviour
{
    [Header("Approach")]
    [SerializeField] float approachTightness = 1f;
    [SerializeField] float keepPressureRange = 0.6f;
    [SerializeField] float minimumApproachFactor = 0.45f;

    [Header("Unpredictability")]
    [SerializeField] float orbitStrength = 0.65f;
    [SerializeField] Vector2 orbitSwitchTimeRange = new Vector2(0.7f, 1.6f);
    [SerializeField] float jitterStrength = 0.35f;
    [SerializeField] Vector2 jitterSwitchTimeRange = new Vector2(0.2f, 0.5f);

    Vector3 jitterDirection;
    float nextJitterChangeTime;
    float nextOrbitSwitchTime;
    int orbitDirectionSign = 1;
    
    public Vector3 ResolveMoveDirection(Vector3 enemyPosition, Transform target, float stopDistance)
    {
        if (target == null)
        {
            return Vector3.zero;
        }

        Vector3 toTarget = target.position - enemyPosition;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;
        if (distance <= 0.001f)
        {
            return Vector3.zero;
        }

        Vector3 approachDirection = toTarget / distance;
        float pressureDistance = Mathf.Max(0.05f, stopDistance + keepPressureRange);
        float distancePastStopDistance = Mathf.Max(0f, distance - stopDistance);
        float pressureFactor = Mathf.Clamp01(distancePastStopDistance / pressureDistance);
        float approachFactor = Mathf.Lerp(minimumApproachFactor, 1f, pressureFactor) * approachTightness;

        UpdateOrbitDirection();
        UpdateJitterDirection();

        Vector3 orbitDirection = Vector3.Cross(Vector3.up, approachDirection) * orbitDirectionSign;
        float randomInfluence = pressureFactor;
        Vector3 desired =
            approachDirection * approachFactor +
            orbitDirection * orbitStrength * randomInfluence +
            jitterDirection * jitterStrength * randomInfluence;

        if (desired.sqrMagnitude <= 0.0001f)
        {
            return Vector3.zero;
        }

        return desired.normalized;
    }

    void UpdateOrbitDirection()
    {
        if (Time.time < nextOrbitSwitchTime)
        {
            return;
        }

        orbitDirectionSign = Random.value > 0.5f ? 1 : -1;
        nextOrbitSwitchTime = Time.time + Random.Range(orbitSwitchTimeRange.x, orbitSwitchTimeRange.y);
    }

    void UpdateJitterDirection()
    {
        if (Time.time < nextJitterChangeTime)
        {
            return;
        }

        Vector2 random = Random.insideUnitCircle.normalized;
        jitterDirection = new Vector3(random.x, 0f, random.y);
        nextJitterChangeTime = Time.time + Random.Range(jitterSwitchTimeRange.x, jitterSwitchTimeRange.y);
    }

    void OnValidate()
    {
        approachTightness = Mathf.Max(0f, approachTightness);
        keepPressureRange = Mathf.Max(0f, keepPressureRange);
        minimumApproachFactor = Mathf.Clamp01(minimumApproachFactor);
        orbitStrength = Mathf.Max(0f, orbitStrength);
        jitterStrength = Mathf.Max(0f, jitterStrength);

        orbitSwitchTimeRange.x = Mathf.Max(0.05f, orbitSwitchTimeRange.x);
        orbitSwitchTimeRange.y = Mathf.Max(orbitSwitchTimeRange.x, orbitSwitchTimeRange.y);
        jitterSwitchTimeRange.x = Mathf.Max(0.05f, jitterSwitchTimeRange.x);
        jitterSwitchTimeRange.y = Mathf.Max(jitterSwitchTimeRange.x, jitterSwitchTimeRange.y);
    }
}
