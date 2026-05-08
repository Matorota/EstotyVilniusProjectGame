using UnityEngine;

public class CharacterDefense : MonoBehaviour
{
    [SerializeField] private CharacterDefendAnimation defendAnimation;
    [SerializeField] private CharacterInputReader inputReader;

    private bool isDefending;

    public bool IsDefending => isDefending;

    private void Awake()
    {
        defendAnimation ??= GetComponent<CharacterDefendAnimation>();
        inputReader ??= GetComponent<CharacterInputReader>();
    }

    private void Update()
    {
        SetDefending(inputReader != null && inputReader.WantsDefense);
    }

    public void SetUiDefense(bool value)
    {
        inputReader?.SetUiDefense(value);
    }

    private void SetDefending(bool value)
    {
        if (isDefending == value)
            return;

        isDefending = value;
        defendAnimation?.SetDefending(value);
    }
}
