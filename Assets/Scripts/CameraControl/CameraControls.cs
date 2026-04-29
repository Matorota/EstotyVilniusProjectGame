using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 12f, -12f);
    [SerializeField] bool followInLateUpdate = true;

    [Header("Optional Rotation Lock")]
    [Tooltip("When enabled, camera rotation is forced to the isometric angles while following.")]
    [SerializeField] bool lockRotationToIsometric = false;
    [SerializeField] Vector3 isometricEulerAngles = new Vector3(35.264f, 45f, 0f);

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        ApplyOptionalRotationLock();
    }

    void LateUpdate()
    {
        if (!followInLateUpdate || target == null)
        {
            return;
        }

        FollowTarget();
    }

    void Update()
    {
        if (followInLateUpdate || target == null)
        {
            return;
        }

        FollowTarget();
    }

    private void FollowTarget()
    {
        transform.position = target.position + offset;
        ApplyOptionalRotationLock();
    }

    private void ApplyOptionalRotationLock()
    {
        if (!lockRotationToIsometric)
        {
            return;
        }

        transform.rotation = Quaternion.Euler(isometricEulerAngles);
    }

    public Transform GetCameraTransform()
    {
        return transform;
    }
}
