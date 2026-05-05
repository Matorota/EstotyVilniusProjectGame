using UnityEngine;

[RequireComponent(typeof(CharacterInputReader))]
[RequireComponent(typeof(MovementDirectionResolver))]
[RequireComponent(typeof(CharacterMotor))]
public class CharacterMovements : MonoBehaviour
{
    CharacterInputReader inputReader;
    MovementDirectionResolver directionResolver;
    CharacterMotor motor;

    private void Awake()
    {
        inputReader = GetComponent<CharacterInputReader>();
        directionResolver = GetComponent<MovementDirectionResolver>();
        motor = GetComponent<CharacterMotor>();
    }

    private void Update()
    {
        Vector2 movementInput = inputReader.ReadMovementInput();
        Vector3 moveDirection = directionResolver.ResolveMoveDirection(movementInput);
        motor.Tick(moveDirection);
    }

    public void ResetVelocity()
    {
        motor.ResetVelocity();
    }
}
