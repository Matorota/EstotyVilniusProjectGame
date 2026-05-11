using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -12f);

    private void Awake()
    {
        if (player == null)
        {
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            enabled = false;
            return;
        }
        transform.position = player.position + offset;
    }
}
