using UnityEngine;

[RequireComponent(typeof(CharacterInputReader))]
[RequireComponent(typeof(MovementDirectionResolver))]
[RequireComponent(typeof(CharacterMotor))]
public class CharacterMovements : MonoBehaviour
{
    public static CharacterMovements MainCharacter { get; private set; }

    CharacterInputReader inputReader;
    MovementDirectionResolver directionResolver;
    CharacterMotor motor;

    void Awake()
    {
        MainCharacter = this;
        inputReader = GetComponent<CharacterInputReader>();
        directionResolver = GetComponent<MovementDirectionResolver>();
        motor = GetComponent<CharacterMotor>();
    }

    void OnDestroy()
    {
        if (MainCharacter == this)
        {
            MainCharacter = null;
        }
    }

    void Update()
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
