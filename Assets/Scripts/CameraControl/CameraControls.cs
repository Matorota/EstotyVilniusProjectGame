using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 12f, -12f);

    void Start()
    {
        if (target == null)
        {
            CharacterMovements player = FindObjectOfType<CharacterMovements>();
            if (player != null) target = player.transform;
        }
    }

    void LateUpdate()
    {
        FollowTargetIfAvailable();
    }

    void FollowTargetIfAvailable()
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
