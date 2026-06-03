using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterDefendAnimation : MonoBehaviour
{
    private const string DefaultDefendBool = "IsDefending";

    [SerializeField] private Animator animator;
    [SerializeField] private string defendBoolParameter = DefaultDefendBool;

    private int defendParameterHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (string.IsNullOrWhiteSpace(defendBoolParameter))
            defendBoolParameter = DefaultDefendBool;

        defendParameterHash = Animator.StringToHash(defendBoolParameter);

        bool found = false;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.nameHash == defendParameterHash)
            {
                found = true;
                break;
            }
        }

        if (!found)
            Debug.LogWarning($"[CharacterDefendAnimation] Animator '{animator.name}' is missing bool parameter '{defendBoolParameter}'.", this);
    }

    public bool IsDefending => animator.GetBool(defendParameterHash);

    public void SetDefending(bool value)
    {
        animator.SetBool(defendParameterHash, value);
    }
}
