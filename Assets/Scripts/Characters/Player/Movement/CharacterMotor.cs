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
            float normalizedSpeed = planarVelocity.magnitude / speed;
            if (normalizedSpeed < 0f)
            {
                return 0f;
            }

            if (normalizedSpeed > 1f)
            {
                return 1f;
            }

            return normalizedSpeed;
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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
            }
        }
        else if (smoothedHorizontalVelocity.sqrMagnitude >= 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(smoothedHorizontalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
        }

        controller.Move(smoothedHorizontalVelocity * Time.deltaTime);
    }

}
