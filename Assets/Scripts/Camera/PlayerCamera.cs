using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -12f);

    private void Awake()
    {
        if (player == null)
            ResolvePlayer();
    }

    private void LateUpdate()
    {
        // If player reference is stale (destroyed), clear it
        if (player != null && player.gameObject == null)
            player = null;

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
        Debug.Log($"[PlayerCamera] Target set to: {(player != null ? player.name : "NULL")}", this);
    }

    private void ResolvePlayer()
    {
        // Try to find player in the scene (from prefab spawn)
        CharacterMovements cm = FindObjectOfType<CharacterMovements>();
        if (cm != null && cm.gameObject != null && cm.gameObject.name == "Player")
        {
            player = cm.transform;
            enabled = true;
            return;
        }

        // Fallback: any CharacterMovements
        if (cm != null && cm.gameObject != null)
        {
            player = cm.transform;
            enabled = true;
            return;
        }

        // Fallback: check parent hierarchy (if camera is child of player)
        Transform current = transform.parent;
        while (current != null)
        {
            cm = current.GetComponent<CharacterMovements>();
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
