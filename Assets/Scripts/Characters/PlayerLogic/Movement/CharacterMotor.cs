using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("Movement")]
    [SerializeField] float gravity = -20f;
    [SerializeField] float speed = 4f;
    [Tooltip("The exact height the player will jump in Unity units (e.g., 1.3 blocks)")]
    [SerializeField] float jumpHeight = 1.3f;
    [SerializeField] float movementSmoothTime = 0.15f;
    [SerializeField] float rotationLerpSpeed = 15f;
    [SerializeField] float groundedVerticalVelocity = -2f;

    Vector3 smoothedHorizontalVelocity;
    Vector3 horizontalVelocitySmoothing;
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

    public void Tick(Vector3 moveDirection, bool jumpRequested)
    {
        ApplyGroundStick();
        ApplyHorizontalMovement(moveDirection);

        if (jumpRequested)
        {
            TryJump();
        }

        ApplyGravity();
        MoveCharacter();
    }

    public void ResetVelocity()
    {
        verticalVelocity = 0f;
        smoothedHorizontalVelocity = Vector3.zero;
        horizontalVelocitySmoothing = Vector3.zero;
    }

    void ApplyGroundStick()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedVerticalVelocity;
        }
    }

    void ApplyHorizontalMovement(Vector3 moveDirection)
    {
        Vector3 targetHorizontalVelocity = moveDirection * speed;
        smoothedHorizontalVelocity = Vector3.SmoothDamp(
            smoothedHorizontalVelocity,
            targetHorizontalVelocity,
            ref horizontalVelocitySmoothing,
            movementSmoothTime
        );

        if (smoothedHorizontalVelocity.sqrMagnitude >= 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(smoothedHorizontalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
        }
    }

    void TryJump()
    {
        if (!controller.isGrounded)
        {
            return;
        }

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
    }

    void MoveCharacter()
    {
        Vector3 frameMovement = smoothedHorizontalVelocity;
        frameMovement.y = verticalVelocity;
        controller.Move(frameMovement * Time.deltaTime);
    }

    void CacheDependencies()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    void ClampValues()
    {
        gravity = Mathf.Min(-0.01f, gravity);
        groundedVerticalVelocity = Mathf.Min(0f, groundedVerticalVelocity);
        speed = Mathf.Max(0f, speed);
        jumpHeight = Mathf.Max(0f, jumpHeight);
        movementSmoothTime = Mathf.Max(0f, movementSmoothTime);
        rotationLerpSpeed = Mathf.Max(0f, rotationLerpSpeed);
    }
}
