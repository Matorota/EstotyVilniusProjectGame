using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAttackAnimation : MonoBehaviour
{
    private const string AttackParameter = "Attack";
    private const string HitParameter = "Hit";

    private static readonly int AttackHash = Animator.StringToHash(AttackParameter);
    private static readonly int HitHash = Animator.StringToHash(HitParameter);

    [SerializeField] private Animator animator;
    [SerializeField] private CharacterDefense defense;

    private bool hasAttackParameter;
    private bool hasHitParameter;

    public bool IsDefending => defense != null && defense.IsDefending;

    private void Awake()
    {
        animator ??= GetComponent<Animator>();
        defense ??= GetComponent<CharacterDefense>();

        CacheAnimatorParameters();
    }

    private void CacheAnimatorParameters()
    {
        if (animator == null)
        {
            return;
        }

        AnimatorControllerParameter[] parameters = animator.parameters;

        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];

            if (parameter.type != AnimatorControllerParameterType.Trigger)
            {
                continue;
            }

            if (parameter.name == AttackParameter)
            {
                hasAttackParameter = true;
            }
            else if (parameter.name == HitParameter)
            {
                hasHitParameter = true;
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
        if (animator == null || !hasAttackParameter)
        {
            return false;
        }

        animator.ResetTrigger(AttackHash);
        animator.SetTrigger(AttackHash);
        return true;
    }

    public void ClearAttackTrigger()
    {
        if (animator == null || !hasAttackParameter)
        {
            return;
        }

        animator.ResetTrigger(AttackHash);
    }

    public void TryPlayHit()
    {
        if (animator == null || !hasHitParameter || IsDefending)
        {
            return;
        }

        animator.SetTrigger(HitHash);
    }
}