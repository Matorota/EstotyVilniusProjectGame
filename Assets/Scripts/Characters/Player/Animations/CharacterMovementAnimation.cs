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

    [SerializeField] private float parameterDampTime = 0.12f;
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

        parameterDampTime = Mathf.Max(0f, parameterDampTime);
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
        float inputSpeed = Mathf.Clamp01(movementInput.magnitude);
        float targetSpeed = Mathf.Max(inputSpeed, Mathf.Clamp01(normalizedSpeed));
        if (hasSpeedParameter)
        {
            animator.SetFloat(speedParameter, targetSpeed, parameterDampTime, Time.deltaTime);
        }

        if (!useDirectionalParameters || !hasMoveXParameter)
        {
            return;
        }

        Vector3 directionSource = worldVelocity.sqrMagnitude > 0.0001f ? worldVelocity : worldMoveDirection;
        Vector3 localDirection = transform.InverseTransformDirection(directionSource);
        float moveX = Mathf.Clamp(localDirection.x, -1f, 1f);

        animator.SetFloat(moveXParameter, moveX, parameterDampTime, Time.deltaTime);
    }
}
