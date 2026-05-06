using UnityEngine;

[RequireComponent(typeof(CharacterInputReader))]
[RequireComponent(typeof(MovementDirectionResolver))]
[RequireComponent(typeof(CharacterMotor))]
public class CharacterMovements : MonoBehaviour
{
    private CharacterInputReader inputReader;
    private MovementDirectionResolver directionResolver;
    private CharacterMotor motor;
    private CharacterMovementAnimation movementAnimation;

    private void Awake()
    {
        inputReader = GetComponent<CharacterInputReader>();
        directionResolver = GetComponent<MovementDirectionResolver>();
        motor = GetComponent<CharacterMotor>();
        movementAnimation = GetComponent<CharacterMovementAnimation>();
    }

    private void Update()
    {
        Vector2 movementInput = inputReader.ReadMovementInput();
        Vector3 moveDirection = directionResolver.ResolveMoveDirection(movementInput);
        motor.Tick(moveDirection);
        if (movementAnimation != null)
        {
            movementAnimation.Tick(movementInput, moveDirection, motor.NormalizedHorizontalSpeed, motor.HorizontalVelocity);
        }
    }

    public void ResetVelocity()
    {
        motor.ResetVelocity();
    }
}
