using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    CharacterController controller;

    [SerializeField] private Transform target;

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

        Vector3 desiredDirection = ResolveDirectMoveDirection();
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

    Vector3 ResolveDirectMoveDirection()
    {
        if (target == null)
        {
            Debug.LogError("You forgot to add target");
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
}
