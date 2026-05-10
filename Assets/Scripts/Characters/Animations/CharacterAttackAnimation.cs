using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAttackAnimation : MonoBehaviour
{
    private const string AttackParameter = "Attack";
    private static readonly int AttackHash = Animator.StringToHash(AttackParameter);

    [SerializeField] private Animator animator;
    [SerializeField] private CharacterDefense defense;

    private bool hasAttackParameter;

    public bool IsDefending => defense != null && defense.IsDefending;

    private void Awake()
    {
        animator ??= GetComponent<Animator>();
        defense ??= GetComponent<CharacterDefense>();

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == AttackParameter)
            {
                hasAttackParameter = true;
                break;
            }
        }
    }

    public bool TryPlayAttack()
    {
        if (IsDefending)
        {
            ClearAttackTrigger();
            return false;
        }

        return ForcePlayAttack();
    }

    public bool ForcePlayAttack()
    {
        if (!hasAttackParameter)
        {
            return false;
        }

        animator.ResetTrigger(AttackHash);
        animator.SetTrigger(AttackHash);
        return true;
    }

    public void ClearAttackTrigger()
    {
        if (!hasAttackParameter)
        {
            return;
        }

        animator.ResetTrigger(AttackHash);
    }
}