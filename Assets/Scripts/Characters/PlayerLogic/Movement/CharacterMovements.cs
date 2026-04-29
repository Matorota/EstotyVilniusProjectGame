using UnityEngine;
using UnityEngine.InputSystem;
using Terresquall;

public class CharacterMovements : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [Header("Input")]
    [SerializeField] VirtualJoystick joystick;
    [SerializeField] int joystickId = 0;
    [SerializeField] bool useJoystick = true;

    [Header("Movement")]
    [SerializeField] float gravityValue = -20f;
    [SerializeField] float speed = 4f;
    [Tooltip("The exact height the player will jump in Unity units (e.g., 1.3 blocks)")]
    [SerializeField] float jumpHeight = 1.3f;
    [SerializeField] float smoothTime = 0.15f;
    [SerializeField] Transform movementCamera;
    [SerializeField] bool useCameraRelativeMovement = true;

    private Vector3 playerVelocity;
    private Vector3 currentMove;
    private Vector3 moveVelocity;
    
    private readonly Vector3 isoForward = new Vector3(1, 0, 1).normalized;
    private readonly Vector3 isoRight = new Vector3(1, 0, -1).normalized;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (movementCamera == null && Camera.main != null)
        {
            movementCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        bool grounded = controller.isGrounded;

        if (grounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        
        Vector2 movementInput = GetMovementInput();

        Vector3 targetMove = GetMoveDirection(movementInput);
        if (targetMove.magnitude > 1f) targetMove.Normalize();
        targetMove *= speed;

        currentMove = Vector3.SmoothDamp(currentMove, targetMove, ref moveVelocity, smoothTime);


        Vector3 finalMovement = currentMove;

        if (currentMove.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            TryJump();
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        finalMovement.y = playerVelocity.y;

        controller.Move(finalMovement * Time.deltaTime);
    }

    private Vector3 GetMoveDirection(Vector2 movementInput)
    {
        if (!useCameraRelativeMovement || movementCamera == null)
        {
            return isoForward * movementInput.y + isoRight * movementInput.x;
        }

        Vector3 cameraForward = movementCamera.forward;
        Vector3 cameraRight = movementCamera.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        if (cameraForward.sqrMagnitude < 0.0001f || cameraRight.sqrMagnitude < 0.0001f)
        {
            return isoForward * movementInput.y + isoRight * movementInput.x;
        }

        cameraForward.Normalize();
        cameraRight.Normalize();

        return cameraForward * movementInput.y + cameraRight * movementInput.x;
    }

    private Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero;

        if (useJoystick)
        {
            if (joystick == null && VirtualJoystick.CountActiveInstances() > 0)
            {
                joystick = VirtualJoystick.GetInstance(joystickId);
            }

            if (joystick != null && joystick.isActiveAndEnabled)
            {
                input = joystick.GetAxis();
            }
        }

        if (input.sqrMagnitude > 0f)
        {
            return Vector2.ClampMagnitude(input, 1f);
        }

        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector2.zero;
        }

        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;

        return Vector2.ClampMagnitude(input, 1f);
    }

    public void Jump()
    {
        TryJump();
    }

    private void TryJump()
    {
        if (!controller.isGrounded)
        {
            return;
        }

        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
    }

    public void ResetVelocity()
    {
        playerVelocity = Vector3.zero;
        currentMove = Vector3.zero;
        moveVelocity = Vector3.zero;
    }
}
