using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField] private Transform target;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 1.1f;
    [SerializeField] private float resumeDistanceBuffer = 0.2f;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private float rotationLerpSpeed = 14f;

    [SerializeField] private float obstacleCheckDistance = 2.5f;
    [SerializeField] private float sideCheckDistance = 1.2f;
    [SerializeField] private LayerMask obstacleMask = ~0;

    private Vector3 currentMove;
    private Vector3 moveVelocity;
    private bool isStoppedByDistance;

    public Vector3 HorizontalVelocity => currentMove;
    public float NormalizedHorizontalSpeed => speed > 0f ? Mathf.Clamp01(currentMove.magnitude / speed) : 0f;

    private float verticalVelocity;
    private const float Gravity = -9.81f;
    private const float GroundStick = -1f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (resumeDistanceBuffer < 0f) resumeDistanceBuffer = 0f;
    }

    private void Update()
    {
        if (target == null)
            return;
        
        Vector3 toTarget = target.position - transform.position;
        toTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up);
        float distanceToTarget = toTarget.magnitude;
        if (isStoppedByDistance)
        {
            if (distanceToTarget > stopDistance + resumeDistanceBuffer)
            {
                isStoppedByDistance = false;
            }
        }
        else if (distanceToTarget <= stopDistance || distanceToTarget <= 0.001f)
        {
            isStoppedByDistance = true;
        }

        Vector3 desiredDirection = isStoppedByDistance ? Vector3.zero : toTarget / distanceToTarget;
        desiredDirection = AvoidObstacles(desiredDirection);
        Vector3 desiredMove = desiredDirection * speed;

        currentMove = Vector3.SmoothDamp(currentMove, desiredMove, ref moveVelocity, smoothTime);

        Vector3 lookDir = desiredDirection.sqrMagnitude > 0.0001f ? desiredDirection : toTarget.normalized;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
        }

        float dt = Time.deltaTime;

        bool grounded = controller.isGrounded;
        if (!grounded)
        {
            grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.25f + controller.skinWidth);
        }

        if (grounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = GroundStick; 
            }
        }
        else
        {
            verticalVelocity += Gravity * dt;
            verticalVelocity = Mathf.Max(verticalVelocity, -50f);
        }

        Vector3 finalMove = currentMove + Vector3.up * verticalVelocity;
        controller.Move(finalMove * dt);

        if (controller.collisionFlags.HasFlag(CollisionFlags.Sides))
        {
            currentMove *= 0.5f;
            moveVelocity *= 0.5f;
        }
    }

    private Vector3 AvoidObstacles(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return direction;

        Vector3 origin = transform.position + Vector3.up * (controller.height * 0.3f);
        float radius = controller.radius * 0.85f;

        if (!Physics.SphereCast(origin, radius, direction, out RaycastHit forwardHit, obstacleCheckDistance, obstacleMask))
            return direction;

        float[] testAngles = { -90f, -60f, -45f, -30f, 30f, 45f, 60f, 90f };
        Vector3 bestDir = direction;
        float bestScore = -999f;

        foreach (float angle in testAngles)
        {
            Vector3 testDir = Quaternion.Euler(0, angle, 0) * direction;
            bool blocked = Physics.SphereCast(origin, radius, testDir, out RaycastHit sideHit, sideCheckDistance, obstacleMask);

            float score = Vector3.Dot(testDir, direction);
            if (!blocked)
                score += 2f; 
            else
                score += sideHit.distance; 

            if (score > bestScore)
            {
                bestScore = score;
                bestDir = testDir;
            }
        }

        if (bestScore < 0f && forwardHit.normal.sqrMagnitude > 0.001f)
        {
            Vector3 slide = Vector3.ProjectOnPlane(direction, forwardHit.normal);
            if (slide.sqrMagnitude > 0.0001f)
                bestDir = slide.normalized;
        }

        return bestDir.normalized;
    }
}
