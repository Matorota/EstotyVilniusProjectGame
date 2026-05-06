using UnityEngine;

public class MovementDirectionResolver : MonoBehaviour
{
    [SerializeField] private Transform movementCamera;
    [SerializeField] private bool useCameraRelativeMovement = true;

    private   Vector3 IsoForward = new Vector3(1f, 0f, 1f).normalized;
    private   Vector3 IsoRight = new Vector3(1f, 0f, -1f).normalized;

    public Vector3 ResolveMoveDirection(Vector2 movementInput)
    {
        if (!useCameraRelativeMovement)
        {
            return ToIsometricDirection(movementInput);
        }

        Vector3 cameraForward = movementCamera.forward;
        Vector3 cameraRight = movementCamera.right;
        cameraForward = Vector3.ProjectOnPlane(cameraForward, Vector3.up);
        cameraRight = Vector3.ProjectOnPlane(cameraRight, Vector3.up);

        if (cameraForward.sqrMagnitude < 0.0001f || cameraRight.sqrMagnitude < 0.0001f)
        {
            return ToIsometricDirection(movementInput);
        }

        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 moveDirection = cameraForward * movementInput.y + cameraRight * movementInput.x;
        return moveDirection.sqrMagnitude > 1f ? moveDirection.normalized : moveDirection;
    }

    private  Vector3 ToIsometricDirection(Vector2 movementInput)
    {
        Vector3 moveDirection = IsoForward * movementInput.y + IsoRight * movementInput.x;
        return moveDirection.sqrMagnitude > 1f ? moveDirection.normalized : moveDirection;
    }
}
