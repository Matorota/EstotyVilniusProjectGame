using UnityEngine;

[RequireComponent(typeof(CharacterInputReader))]
[RequireComponent(typeof(MovementDirectionResolver))]
[RequireComponent(typeof(CharacterMotor))]
public class CharacterMovements : MonoBehaviour
{
    [SerializeField] CharacterInputReader inputReader;
    [SerializeField] MovementDirectionResolver directionResolver;
    [SerializeField] CharacterMotor motor;

    void Awake()
    {
        CacheDependencies();
    }

    void OnValidate()
    {
        CacheDependencies();
    }

    void Update()
    {
        Vector2 movementInput = inputReader.ReadMovementInput();
        Vector3 moveDirection = directionResolver.ResolveMoveDirection(movementInput);
        bool jumpRequested = inputReader.ConsumeJumpPressed();
        motor.Tick(moveDirection, jumpRequested);
    }

    public void Jump()
    {
        inputReader.QueueJump();
    }
    public void ResetVelocity()
    {
        motor.ResetVelocity();
    }

    void CacheDependencies()
    {
        if (inputReader == null)
        {
            inputReader = GetComponent<CharacterInputReader>();
        }

        if (directionResolver == null)
        {
            directionResolver = GetComponent<MovementDirectionResolver>();
        }

        if (motor == null)
        {
            motor = GetComponent<CharacterMotor>();
        }
    }
}
