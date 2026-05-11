using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField] private float speed = 4f;
    [SerializeField] private float movementSmoothTime = 0.08f;
    [SerializeField] private float rotationLerpSpeed = 15f;

    private Vector3 smoothedHorizontalVelocity;
    private Vector3 horizontalVelocitySmoothing;
    private Transform facingTarget;

    private float verticalVelocity;
    private const float Gravity = -9.81f;
    private const float GroundStick = -1f;
    private const float MaxFallSpeed = 50f;

    public Vector3 HorizontalVelocity => smoothedHorizontalVelocity;
    public float NormalizedHorizontalSpeed
    {
        get
        {
            if (speed <= 0f) return 0f;
            Vector2 planarVelocity = new Vector2(smoothedHorizontalVelocity.x, smoothedHorizontalVelocity.z);
            return Mathf.Clamp01(planarVelocity.magnitude / speed);
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void SetFacingTarget(Transform target)
    {
        facingTarget = target;
    }

    public void Tick(Vector3 moveDirection)
    {
        float dt = Time.deltaTime;
        Vector3 targetHorizontalVelocity = moveDirection * speed;
        smoothedHorizontalVelocity = Vector3.SmoothDamp(
            smoothedHorizontalVelocity,
            targetHorizontalVelocity,
            ref horizontalVelocitySmoothing,
            movementSmoothTime
        );

        if (facingTarget != null)
        {
            Vector3 toTarget = facingTarget.position - transform.position;
            toTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up);
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * dt);
            }
        }
        else if (smoothedHorizontalVelocity.sqrMagnitude >= 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(smoothedHorizontalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * dt);
        }

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = GroundStick;
        }
        else
        {
            verticalVelocity += Gravity * dt;
            verticalVelocity = Mathf.Max(verticalVelocity, -MaxFallSpeed);
        }

        Vector3 finalMove = smoothedHorizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalMove * dt);
    }

}
