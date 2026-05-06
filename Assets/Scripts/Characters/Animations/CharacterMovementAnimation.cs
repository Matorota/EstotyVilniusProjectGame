using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMovementAnimation : MonoBehaviour
{
    private const string DefaultSpeedParameter = "Speed";
    private const string DefaultMoveXParameter = "MoveX";

    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = DefaultSpeedParameter;
    [SerializeField] private string moveXParameter = DefaultMoveXParameter;
    [SerializeField] private bool useDirectionalParameters = true;

    [SerializeField] private float parameterDampTime = 0.05f;
    [SerializeField] private float minimumMovingAnimationSpeed = 0.25f;
    private bool hasSpeedParameter;
    private bool hasMoveXParameter;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (string.IsNullOrWhiteSpace(speedParameter))
        {
            speedParameter = DefaultSpeedParameter;
        }

        if (string.IsNullOrWhiteSpace(moveXParameter))
        {
            moveXParameter = DefaultMoveXParameter;
        }

        if (parameterDampTime < 0f)
        {
            parameterDampTime = 0f;
        }
        if (minimumMovingAnimationSpeed < 0f)
        {
            minimumMovingAnimationSpeed = 0f;
        }
        else if (minimumMovingAnimationSpeed > 1f)
        {
            minimumMovingAnimationSpeed = 1f;
        }
        hasSpeedParameter = false;
        hasMoveXParameter = false;

        AnimatorControllerParameter[] parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            if (parameter.type != AnimatorControllerParameterType.Float)
            {
                continue;
            }

            if (parameter.name == speedParameter)
            {
                hasSpeedParameter = true;
            }
            else if (parameter.name == moveXParameter)
            {
                hasMoveXParameter = true;
            }
        }
    }

    public void Tick(Vector2 movementInput, Vector3 worldMoveDirection, float normalizedSpeed, Vector3 worldVelocity)
    {
        float inputSpeed = movementInput.magnitude;
        if (inputSpeed > 1f)
        {
            inputSpeed = 1f;
        }

        float cappedNormalizedSpeed = normalizedSpeed;
        if (cappedNormalizedSpeed < 0f)
        {
            cappedNormalizedSpeed = 0f;
        }
        else if (cappedNormalizedSpeed > 1f)
        {
            cappedNormalizedSpeed = 1f;
        }

        bool hasMovementInput = movementInput.sqrMagnitude > 0.0001f || worldMoveDirection.sqrMagnitude > 0.0001f;
        float targetSpeed = inputSpeed > cappedNormalizedSpeed ? inputSpeed : cappedNormalizedSpeed;
        if (hasMovementInput && targetSpeed < minimumMovingAnimationSpeed)
        {
            targetSpeed = minimumMovingAnimationSpeed;
        }
        else if (!hasMovementInput)
        {
            targetSpeed = 0f;
        }

        if (hasSpeedParameter)
        {
            animator.SetFloat(speedParameter, targetSpeed);
        }

        if (!useDirectionalParameters || !hasMoveXParameter)
        {
            return;
        }

        Vector3 directionSource = worldVelocity.sqrMagnitude > 0.0001f ? worldVelocity : worldMoveDirection;
        Vector3 localDirection = transform.InverseTransformDirection(directionSource);
        float moveX = localDirection.x;
        if (moveX < -1f)
        {
            moveX = -1f;
        }
        else if (moveX > 1f)
        {
            moveX = 1f;
        }

        animator.SetFloat(moveXParameter, moveX, parameterDampTime, Time.deltaTime);
    }
}
