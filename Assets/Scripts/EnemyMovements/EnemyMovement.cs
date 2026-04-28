using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float gravityValue = -20f;
    [SerializeField] float speed = 3f;
    [Tooltip("The exact height the player will jump in Unity units (e.g., 1.3 blocks)")]
    [SerializeField] float jumpHeight = 1.3f;
    [SerializeField] float smoothTime = 0.15f;
    
    private Vector3 playerVelocity;
    private Vector3 currentMove;
    private Vector3 moveVelocity;

    private readonly Vector3 isoForward = new Vector3(1, 0, 1).normalized;
    private readonly Vector3 isoRight = new Vector3(1, 0, -1).normalized;
    
    void Start()
    {
            
    }

    void Update()
    {
        
    }
}
