using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField] private Transform target;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 1.1f;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private float rotationLerpSpeed = 14f;

    private Vector3 currentMove;
    private Vector3 moveVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }
        
        Vector3 toTarget = target.position - transform.position;
        toTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up);
        float distanceToTarget = toTarget.magnitude;
        Vector3 desiredDirection =
            (distanceToTarget <= stopDistance || distanceToTarget <= 0.001f)
            ? Vector3.zero
            : toTarget / distanceToTarget;
        Vector3 desiredMove = desiredDirection * speed;

        currentMove = Vector3.SmoothDamp(currentMove, desiredMove, ref moveVelocity, smoothTime);
        if (currentMove.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
        }

        controller.Move(currentMove * Time.deltaTime);
    }
}
