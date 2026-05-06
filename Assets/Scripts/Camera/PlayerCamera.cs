using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -12f);

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        transform.position = player.position + offset;
    }
}
