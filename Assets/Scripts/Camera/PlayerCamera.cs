using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset = new Vector3(0f, 12f, -12f);

    private void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
