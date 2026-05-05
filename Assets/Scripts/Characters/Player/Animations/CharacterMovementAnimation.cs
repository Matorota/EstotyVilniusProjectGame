using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMovementAnimation : MonoBehaviour
{
    const string DefaultSpeedParameter = "Speed";
    const string DefaultMoveXParameter = "MoveX";
    const string DefaultMoveYParameter = "MoveY";

    [Header("Animator")]
    [SerializeField] Animator animator;
    [SerializeField] string speedParameter = DefaultSpeedParameter;
    [SerializeField] string moveXParameter = DefaultMoveXParameter;
    [SerializeField] string moveYParameter = DefaultMoveYParameter;
    [SerializeField] bool useDirectionalParameters = true;

    [Header("Smoothing")]
    [SerializeField] float parameterDampTime = 0.12f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        NormalizeValues();
    }

    public void Tick(Vector2 movementInput, Vector3 worldMoveDirection, float normalizedSpeed, Vector3 worldVelocity)
    {
        float inputSpeed = Mathf.Clamp01(movementInput.magnitude);
        float targetSpeed = Mathf.Max(inputSpeed, Mathf.Clamp01(normalizedSpeed));
        animator.SetFloat(speedParameter, targetSpeed, parameterDampTime, Time.deltaTime);

        if (!useDirectionalParameters)
        {
            return;
        }

        Vector3 directionSource = worldVelocity.sqrMagnitude > 0.0001f ? worldVelocity : worldMoveDirection;
        Vector3 localDirection = transform.InverseTransformDirection(directionSource);
        float moveX = Mathf.Clamp(localDirection.x, -1f, 1f);
        float moveY = Mathf.Clamp(localDirection.z, -1f, 1f);

        animator.SetFloat(moveXParameter, moveX, parameterDampTime, Time.deltaTime);
        animator.SetFloat(moveYParameter, moveY, parameterDampTime, Time.deltaTime);
    }

    void NormalizeValues()
    {
        if (string.IsNullOrWhiteSpace(speedParameter))
        {
            speedParameter = DefaultSpeedParameter;
        }

        if (string.IsNullOrWhiteSpace(moveXParameter))
        {
            moveXParameter = DefaultMoveXParameter;
        }

        if (string.IsNullOrWhiteSpace(moveYParameter))
        {
            moveYParameter = DefaultMoveYParameter;
        }

        parameterDampTime = Mathf.Max(0f, parameterDampTime);
    }
}
