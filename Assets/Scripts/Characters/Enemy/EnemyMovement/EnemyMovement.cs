using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    private CharacterController controller;
    private CharacterMovementAnimation movementAnimation;

    [SerializeField] private Transform target;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 1.1f;
    [SerializeField] private float resumeDistanceBuffer = 0.2f;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private float rotationLerpSpeed = 14f;

    private Vector3 currentMove;
    private Vector3 moveVelocity;
    private bool isStoppedByDistance;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        movementAnimation = GetComponent<CharacterMovementAnimation>();
        if (resumeDistanceBuffer < 0f)
        {
            resumeDistanceBuffer = 0f;
        }
    }

    private void Update()
    {
        if (target == null)
        {
            if (movementAnimation != null)
            {
                movementAnimation.Tick(Vector2.zero, Vector3.zero, 0f, Vector3.zero);
            }
            return;
        }
        
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
        Vector3 desiredMove = desiredDirection * speed;

        currentMove = Vector3.SmoothDamp(currentMove, desiredMove, ref moveVelocity, smoothTime);
        if (currentMove.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
        }

        controller.Move(currentMove * Time.deltaTime);

        if (movementAnimation == null)
        {
            return;
        }

        Vector3 animationDirection = currentMove.sqrMagnitude > 0.0001f ? currentMove : desiredDirection;
        Vector2 movementInput = new Vector2(currentMove.x, currentMove.z);
        float normalizedSpeed = speed > 0f ? currentMove.magnitude / speed : 0f;
        if (normalizedSpeed < 0f)
        {
            normalizedSpeed = 0f;
        }
        else if (normalizedSpeed > 1f)
        {
            normalizedSpeed = 1f;
        }

        movementAnimation.Tick(movementInput, animationDirection, normalizedSpeed, currentMove);
    }
}
