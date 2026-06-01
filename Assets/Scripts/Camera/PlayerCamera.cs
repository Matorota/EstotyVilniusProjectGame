using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerLifecycle playerLifecycle;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -12f);

    private void Awake()
    {
        if (player == null && playerLifecycle != null)
            player = playerLifecycle.transform;

        if (player == null)
            ResolvePlayer();
    }

    private void LateUpdate()
    {
        if (player == null && playerLifecycle != null)
            player = playerLifecycle.transform;

        if (player == null)
            ResolvePlayer();

        if (player == null)
            return;

        transform.position = player.position + offset;
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    private void ResolvePlayer()
    {
        PlayerLifecycle lifecycle = FindFirstObjectByType<PlayerLifecycle>();
        if (lifecycle != null && lifecycle.gameObject != null)
        {
            player = lifecycle.transform;
            return;
        }

        Transform current = transform.parent;
        while (current != null)
        {
            CharacterMovements cm = current.GetComponent<CharacterMovements>();
            if (cm != null)
            {
                player = cm.transform;
                return;
            }
            current = current.parent;
        }
    }
}
