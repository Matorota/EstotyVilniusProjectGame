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
    private CharacterMeleeAttack characterMeleeAttack;
    public bool IsMoving { get; private set; }

    private void Awake()
    {
        inputReader = GetComponent<CharacterInputReader>(); 
        directionResolver = GetComponent<MovementDirectionResolver>();
        motor = GetComponent<CharacterMotor>();
        movementAnimation = GetComponent<CharacterMovementAnimation>();
        characterMeleeAttack = GetComponent<CharacterMeleeAttack>();

    }

    private void Update()
    {
        Vector2 movementInput = inputReader.MovementInput;
        IsMoving = movementInput.sqrMagnitude > 0.0001f;
        Vector3 moveDirection = directionResolver.ResolveMoveDirection(movementInput);
        motor.SetFacingTarget(characterMeleeAttack != null ? characterMeleeAttack.CurrentTargetTransform : null);
        motor.Tick(moveDirection);
        if (movementAnimation != null)
        {
            movementAnimation.Tick(movementInput, moveDirection, motor.NormalizedHorizontalSpeed, motor.HorizontalVelocity);
        }
    }
}
