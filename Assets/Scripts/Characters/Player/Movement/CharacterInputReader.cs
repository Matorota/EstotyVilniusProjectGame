using Terresquall;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class CharacterInputReader : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private bool useJoystick = true;
    private Vector2 movementInput;
    private bool mouseDefenseHeld;
    private bool uiDefenseHeld;

    public Vector2 MovementInput => movementInput;
    public bool WantsDefense => mouseDefenseHeld || uiDefenseHeld;

    private void Update()
    {
        movementInput = ReadMovementInputInternal();
        mouseDefenseHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;
    }

    public Vector2 ReadMovementInput()
    {
        return movementInput;
    }

    public void ToggleUiDefense()
    {
        uiDefenseHeld = !uiDefenseHeld;
    }

    public void SetUiDefense(bool value)
    {
        uiDefenseHeld = value;
    }

    private Vector2 ReadMovementInputInternal()
    {
        if (useJoystick && joystick.isActiveAndEnabled)
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
