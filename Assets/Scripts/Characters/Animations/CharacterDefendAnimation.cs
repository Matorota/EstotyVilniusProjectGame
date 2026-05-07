using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterDefendAnimation : MonoBehaviour
{
    private const string DefaultDefendBool = "IsDefending";

    [SerializeField] private Animator animator;
    [SerializeField] private string defendBoolParameter = DefaultDefendBool;

    private bool hasDefendParameter;
    private int defendParameterHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (string.IsNullOrWhiteSpace(defendBoolParameter))
        {
            defendBoolParameter = DefaultDefendBool;
        }

        hasDefendParameter = false;
        defendParameterHash = Animator.StringToHash(defendBoolParameter);

        AnimatorControllerParameter[] parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name == defendBoolParameter)
            {
                hasDefendParameter = true;
                break;
            }
        }
    }

    public bool IsDefending => hasDefendParameter ? animator.GetBool(defendParameterHash) : false;

    public void SetDefending(bool value)
    {
        if (!hasDefendParameter)
        {
            return;
        }

        animator.SetBool(defendParameterHash, value);
    }
}
