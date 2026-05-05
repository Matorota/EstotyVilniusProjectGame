using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(EnemyTargetTracker))]
public class EnemyMovement : MonoBehaviour
{
    CharacterController controller;
    EnemyTargetTracker targetTracker;

    [Header("Movement")]
    [SerializeField] float speed = 3f;
    [SerializeField] float stopDistance = 1.1f;
    [SerializeField] float smoothTime = 0.12f;
    [SerializeField] float rotationLerpSpeed = 14f;

    [Header("Vertical")]
    [SerializeField] float gravityValue = -20f;
    [SerializeField] float groundedVerticalVelocity = -2f;

    Vector3 currentMove;
    Vector3 moveVelocity;
    float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        targetTracker = GetComponent<EnemyTargetTracker>();
        ClampValues();
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedVerticalVelocity;
        }

        Transform target = targetTracker.ResolveTarget();
        Vector3 desiredDirection = ResolveDirectMoveDirection(target);
        Vector3 desiredMove = desiredDirection * speed;

        currentMove = Vector3.SmoothDamp(currentMove, desiredMove, ref moveVelocity, smoothTime);
        if (currentMove.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
        }

        verticalVelocity += gravityValue * Time.deltaTime;
        Vector3 frameMovement = currentMove;
        frameMovement.y = verticalVelocity;
        controller.Move(frameMovement * Time.deltaTime);
    }

    Vector3 ResolveDirectMoveDirection(Transform target)
    {
        if (target == null)
        {
            return Vector3.zero;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float distanceToTarget = toTarget.magnitude;
        if (distanceToTarget <= stopDistance || distanceToTarget <= 0.001f)
        {
            return Vector3.zero;
        }

        return toTarget / distanceToTarget;
    }

    private void ClampValues()
    {
        speed = Mathf.Max(0f, speed);
        stopDistance = Mathf.Max(0f, stopDistance);
        smoothTime = Mathf.Max(0f, smoothTime);
        rotationLerpSpeed = Mathf.Max(0f, rotationLerpSpeed);
        gravityValue = Mathf.Min(-0.01f, gravityValue);
        groundedVerticalVelocity = Mathf.Min(0f, groundedVerticalVelocity);
    }
}
