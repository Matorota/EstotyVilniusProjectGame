using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -12f);

    private void Awake()
    {
        Instance = this;
        if (player == null)
            ResolvePlayer();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LateUpdate()
    {
        if (player == null)
            ResolvePlayer();

        if (player == null)
        {
            enabled = false;
            return;
        }

        transform.position = player.position + offset;
    }

    public void SetTarget(Transform target)
    {
        player = target;
        if (player != null)
            enabled = true;
    }

    private void ResolvePlayer()
    {
        if (PlayerLifecycle.Instance != null)
        {
            player = PlayerLifecycle.Instance.transform;
            enabled = true;
            return;
        }

        Transform current = transform.parent;
        while (current != null)
        {
            CharacterMovements cm = current.GetComponent<CharacterMovements>();
            if (cm != null)
            {
                player = cm.transform;
                enabled = true;
                return;
            }
            current = current.parent;
        }
    }
}
