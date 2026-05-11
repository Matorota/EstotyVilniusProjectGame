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

    private float verticalVelocity;
    private const float Gravity = -9.81f;
    private const float GroundStick = -1f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        movementAnimation = GetComponent<CharacterMovementAnimation>();
        if (resumeDistanceBuffer < 0f) resumeDistanceBuffer = 0f;
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
        if (toTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized);
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

        if (movementAnimation == null)
        {
            return;
        }

        Vector3 animationDirection = desiredDirection;
        Vector2 movementInput = new Vector2(desiredDirection.x, desiredDirection.z);
        float normalizedSpeed = speed > 0f ? desiredMove.magnitude / speed : 0f;
        normalizedSpeed = Mathf.Clamp01(normalizedSpeed);

        movementAnimation.Tick(movementInput, animationDirection, normalizedSpeed, currentMove);
    }
}
