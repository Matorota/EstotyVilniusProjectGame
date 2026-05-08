using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAttackAnimation : MonoBehaviour
{
    private const string DefaultAttackTrigger = "Attack";
    private const string DefaultHitTrigger = "Hit";

    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerParameter = DefaultAttackTrigger;
    [SerializeField] private string hitTriggerParameter = DefaultHitTrigger;
    [SerializeField] private CharacterDefendAnimation defendAnimation;

    private bool hasAttackTriggerParameter;
    private bool hasHitTriggerParameter;
    private int hitTriggerHash;
    public bool IsHitWindowOpen { get; private set; }
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (string.IsNullOrWhiteSpace(attackTriggerParameter))
        {
            attackTriggerParameter = DefaultAttackTrigger;
        }

        if (string.IsNullOrWhiteSpace(hitTriggerParameter))
        {
            hitTriggerParameter = DefaultHitTrigger;
        }

        hasAttackTriggerParameter = false;
        hasHitTriggerParameter = false;
        hitTriggerHash = Animator.StringToHash(hitTriggerParameter);

        AnimatorControllerParameter[] parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == attackTriggerParameter)
            {
                hasAttackTriggerParameter = true;
            }
            else if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == hitTriggerParameter)
            {
                hasHitTriggerParameter = true;
            }
        }
    }

    public bool IsDefending => defendAnimation?.IsDefending == true;

    public void TryPlayAttack()
    {
        if (!hasAttackTriggerParameter)
        {
            return;
        }

        IsHitWindowOpen = false;
        animator.SetTrigger(attackTriggerParameter);
    }

    public void TryPlayHit()
    {
        if (!hasHitTriggerParameter)
        {
            return;
        }

        animator.SetTrigger(hitTriggerHash);
    }
}
