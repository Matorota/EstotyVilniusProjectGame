using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    CharacterController controller;

    [Header("Movement")]
    [SerializeField] float gravity = -20f;
    [SerializeField] float speed = 4f;
    [SerializeField] float movementSmoothTime = 0.15f;
    [SerializeField] float rotationLerpSpeed = 15f;
    [SerializeField] float groundedVerticalVelocity = -2f;

    Vector3 smoothedHorizontalVelocity;
    Vector3 horizontalVelocitySmoothing;
    float verticalVelocity;

    public Vector3 HorizontalVelocity => smoothedHorizontalVelocity;
    public float NormalizedHorizontalSpeed
    {
        get
        {
            if (speed <= 0f)
            {
                return 0f;
            }

            Vector2 planarVelocity = new Vector2(smoothedHorizontalVelocity.x, smoothedHorizontalVelocity.z);
            return Mathf.Clamp01(planarVelocity.magnitude / speed);
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        ClampValues();
    }

    public void Tick(Vector3 moveDirection)
    {
        ApplyGroundStick();
        ApplyHorizontalMovement(moveDirection);
        ApplyGravity();
        MoveCharacter();
    }

    public void ResetVelocity()
    {
        verticalVelocity = 0f;
        smoothedHorizontalVelocity = Vector3.zero;
        horizontalVelocitySmoothing = Vector3.zero;
    }

    private void ApplyGroundStick()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedVerticalVelocity;
        }
    }

    private void ApplyHorizontalMovement(Vector3 moveDirection)
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

    private void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
    }

    private void MoveCharacter()
    {
        Vector3 frameMovement = smoothedHorizontalVelocity;
        frameMovement.y = verticalVelocity;
        controller.Move(frameMovement * Time.deltaTime);
    }

    private void ClampValues()
    {
        gravity = Mathf.Min(-0.01f, gravity);
        groundedVerticalVelocity = Mathf.Min(0f, groundedVerticalVelocity);
        speed = Mathf.Max(0f, speed);
        movementSmoothTime = Mathf.Max(0f, movementSmoothTime);
        rotationLerpSpeed = Mathf.Max(0f, rotationLerpSpeed);
    }
}
