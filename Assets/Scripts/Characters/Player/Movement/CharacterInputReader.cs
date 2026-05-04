using Terresquall;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputReader : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] VirtualJoystick joystick;
    [SerializeField] int joystickId;
    [SerializeField] bool useJoystick = true;

    bool queuedJump;

    public Vector2 ReadMovementInput()
    {
        Vector2 joystickInput = ReadJoystickInput();
        if (joystickInput.sqrMagnitude > 0.0001f)
        {
            return joystickInput;
        }

        return ReadKeyboardInput();
    }

    public bool ConsumeJumpPressed()
    {
        bool keyboardJumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool shouldJump = keyboardJumpPressed || queuedJump;
        queuedJump = false;
        return shouldJump;
    }

    public void QueueJump()
    {
        queuedJump = true;
    }

    Vector2 ReadJoystickInput()
    {
        if (!useJoystick)
        {
            return Vector2.zero;
        }

        if (joystick == null && VirtualJoystick.CountActiveInstances() > 0)
        {
            joystick = VirtualJoystick.GetInstance(joystickId);
        }

        if (joystick != null && joystick.isActiveAndEnabled)
        {
            return Vector2.ClampMagnitude(joystick.GetAxis(), 1f);
        }

        return Vector2.zero;
    }

    static Vector2 ReadKeyboardInput()
    {
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
