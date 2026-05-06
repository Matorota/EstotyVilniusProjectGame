using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAttackAnimation : MonoBehaviour
{
    private const string DefaultAttackTrigger = "Attack";

    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerParameter = DefaultAttackTrigger;

    private bool hasAttackTriggerParameter;
    public bool IsHitWindowOpen { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (string.IsNullOrWhiteSpace(attackTriggerParameter))
        {
            attackTriggerParameter = DefaultAttackTrigger;
        }

        hasAttackTriggerParameter = false;

        AnimatorControllerParameter[] parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == attackTriggerParameter)
            {
                hasAttackTriggerParameter = true;
                break;
            }
        }
    }

    public void TryPlayAttack()
    {
        if (!hasAttackTriggerParameter)
        {
            return;
        }

        IsHitWindowOpen = false;
        animator.SetTrigger(attackTriggerParameter);
    }

    // Will use them dont delete
    public void OnAttackHitStart()
    {
        IsHitWindowOpen = true;
    }

    public void OnAttackHitEnd()
    {
        IsHitWindowOpen = false;
    }
}
