using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] CharacterMovements mainCharacter;
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 12f, -12f);

    private void Start()
    {
        if (target == null && mainCharacter != null)
        {
            target = mainCharacter.transform;
        }
    }

    private void LateUpdate()
    {
        FollowTargetIfAvailable();
    }

    private void FollowTargetIfAvailable()
    {
        if (target == null)
        {
            return;
        }

        FollowTarget();
    }

    private void FollowTarget()
    {
        transform.position = target.position + offset;
    }

    public Transform GetCameraTransform()
    {
        return transform;
    }
}
