using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(EnemyTargetTracker))]
[RequireComponent(typeof(EnemySteeringRandomizer))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] EnemyTargetTracker targetTracker;
    [SerializeField] EnemySteeringRandomizer steeringRandomizer;

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

    void Awake()
    {
        CacheDependencies();
        ClampValues();
    }

    void OnValidate()
    {
        CacheDependencies();
        ClampValues();
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedVerticalVelocity;
        }

        Transform target = targetTracker.ResolveTarget();
        Vector3 desiredDirection = steeringRandomizer.ResolveMoveDirection(transform.position, target, stopDistance);
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

    void CacheDependencies()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if (targetTracker == null)
        {
            targetTracker = GetComponent<EnemyTargetTracker>();
        }

        if (steeringRandomizer == null)
        {
            steeringRandomizer = GetComponent<EnemySteeringRandomizer>();
        }
    }

    void ClampValues()
    {
        speed = Mathf.Max(0f, speed);
        stopDistance = Mathf.Max(0f, stopDistance);
        smoothTime = Mathf.Max(0f, smoothTime);
        rotationLerpSpeed = Mathf.Max(0f, rotationLerpSpeed);
        gravityValue = Mathf.Min(-0.01f, gravityValue);
        groundedVerticalVelocity = Mathf.Min(0f, groundedVerticalVelocity);
    }
}
