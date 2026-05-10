using Terresquall;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class CharacterInputReader : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private bool useJoystick = true;

    private Vector2 movementInput;
    private bool uiDefenseHeld;

    public Vector2 MovementInput => movementInput;
    public bool WantsDefense => uiDefenseHeld || Mouse.current != null && Mouse.current.rightButton.isPressed;

    private void Update()
    {
        movementInput = ReadMovementInput();
    }

    public void SetUiDefense(bool value)
    {
        uiDefenseHeld = value;
    }

    private Vector2 ReadMovementInput()
    {
        if (useJoystick && joystick != null && joystick.isActiveAndEnabled)
        {
            Vector2 joystickInput = Vector2.ClampMagnitude(joystick.GetAxis(), 1f);

            if (joystickInput.sqrMagnitude > 0.0001f)
            {
                return joystickInput;
            }
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return Vector2.zero;
        }

        Vector2 input = Vector2.zero;

        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;

        return Vector2.ClampMagnitude(input, 1f);
    }
}