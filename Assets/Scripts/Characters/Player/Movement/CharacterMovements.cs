using UnityEngine;

[RequireComponent(typeof(CharacterInputReader))]
[RequireComponent(typeof(MovementDirectionResolver))]
[RequireComponent(typeof(CharacterMotor))]
public class CharacterMovements : MonoBehaviour
{
    private CharacterInputReader inputReader;
    private MovementDirectionResolver directionResolver;
    private CharacterMotor motor;
    private CharacterMeleeAttack characterMeleeAttack;
    private Health health;
    public bool IsMoving { get; private set; }

    private void Awake()
    {
        inputReader = GetComponent<CharacterInputReader>();
        directionResolver = GetComponent<MovementDirectionResolver>();
        motor = GetComponent<CharacterMotor>();
        characterMeleeAttack = GetComponent<CharacterMeleeAttack>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health != null && health.CurrentHealth <= 0f)
        {
            IsMoving = false;
            motor.ResetMotion();
            return;
        }

        Vector2 movementInput = inputReader.MovementInput;
        IsMoving = movementInput.sqrMagnitude > 0.0001f;
        Vector3 moveDirection = directionResolver.ResolveMoveDirection(movementInput);
        motor.SetFacingTarget(characterMeleeAttack != null ? characterMeleeAttack.CurrentTargetTransform : null);
        motor.Tick(moveDirection);
    }
}
