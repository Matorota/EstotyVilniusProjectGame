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

    private int speedHash;
    private int moveXHash;
    private CharacterMotor motor;
    private EnemyMovement enemyMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        motor = GetComponent<CharacterMotor>();
        enemyMovement = GetComponent<EnemyMovement>();

        if (string.IsNullOrWhiteSpace(speedParameter))
            speedParameter = DefaultSpeedParameter;
        if (string.IsNullOrWhiteSpace(moveXParameter))
            moveXParameter = DefaultMoveXParameter;

        parameterDampTime = Mathf.Max(0f, parameterDampTime);
        minimumMovingAnimationSpeed = Mathf.Clamp01(minimumMovingAnimationSpeed);

        speedHash = Animator.StringToHash(speedParameter);
        moveXHash = Animator.StringToHash(moveXParameter);

        if (animator != null)
            ValidateParameters();
    }

    private void ValidateParameters()
    {
        bool hasSpeed = false;
        bool hasMoveX = false;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type != AnimatorControllerParameterType.Float)
                continue;
            if (parameter.nameHash == speedHash)
                hasSpeed = true;
            else if (parameter.nameHash == moveXHash)
                hasMoveX = true;
        }

        if (!hasSpeed)
            Debug.LogWarning($"[CharacterMovementAnimation] Animator '{animator.name}' is missing float parameter '{speedParameter}'.", this);
        if (useDirectionalParameters && !hasMoveX)
            Debug.LogWarning($"[CharacterMovementAnimation] Animator '{animator.name}' is missing float parameter '{moveXParameter}'.", this);
    }

    private void LateUpdate()
    {
        if (animator == null)
            return;

        Vector3 velocity = Vector3.zero;
        float normalizedSpeed = 0f;
        bool hasSource = false;

        if (motor != null)
        {
            velocity = motor.HorizontalVelocity;
            normalizedSpeed = motor.NormalizedHorizontalSpeed;
            hasSource = true;
        }
        else if (enemyMovement != null)
        {
            velocity = enemyMovement.HorizontalVelocity;
            normalizedSpeed = enemyMovement.NormalizedHorizontalSpeed;
            hasSource = true;
        }

        if (!hasSource)
            return;

        bool isMoving = velocity.sqrMagnitude > 0.0001f;
        float targetSpeed = isMoving ? Mathf.Max(normalizedSpeed, minimumMovingAnimationSpeed) : 0f;
        animator.SetFloat(speedHash, Mathf.Clamp01(targetSpeed));

        if (!useDirectionalParameters)
            return;

        float moveX = 0f;
        if (velocity.sqrMagnitude > 0.000001f)
        {
            Vector3 localDirection = transform.InverseTransformDirection(velocity.normalized);
            moveX = Mathf.Clamp(localDirection.x, -1f, 1f);
        }

        animator.SetFloat(moveXHash, moveX, parameterDampTime, Time.deltaTime);
    }
}
