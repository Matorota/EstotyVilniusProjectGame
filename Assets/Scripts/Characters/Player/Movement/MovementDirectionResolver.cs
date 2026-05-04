using UnityEngine;

public class MovementDirectionResolver : MonoBehaviour
{
    [Header("Direction")]
    [SerializeField] Transform movementCamera;
    [SerializeField] bool useCameraRelativeMovement = true;

    static readonly Vector3 IsoForward = new Vector3(1f, 0f, 1f).normalized;
    static readonly Vector3 IsoRight = new Vector3(1f, 0f, -1f).normalized;

    private void Start()
    {
        if (movementCamera == null && Camera.main != null)
        {
            movementCamera = Camera.main.transform;
        }
    }

    public Vector3 ResolveMoveDirection(Vector2 movementInput)
    {
        if (!useCameraRelativeMovement || movementCamera == null)
        {
            return ToIsometricDirection(movementInput);
        }

        Vector3 cameraForward = movementCamera.forward;
        Vector3 cameraRight = movementCamera.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        if (cameraForward.sqrMagnitude < 0.0001f || cameraRight.sqrMagnitude < 0.0001f)
        {
            return ToIsometricDirection(movementInput);
        }

        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 moveDirection = cameraForward * movementInput.y + cameraRight * movementInput.x;
        return moveDirection.sqrMagnitude > 1f ? moveDirection.normalized : moveDirection;
    }

    static Vector3 ToIsometricDirection(Vector2 movementInput)
    {
        Vector3 moveDirection = IsoForward * movementInput.y + IsoRight * movementInput.x;
        return moveDirection.sqrMagnitude > 1f ? moveDirection.normalized : moveDirection;
    }
}
