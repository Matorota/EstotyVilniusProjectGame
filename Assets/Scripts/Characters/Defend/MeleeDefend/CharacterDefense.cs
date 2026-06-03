using UnityEngine;

public class CharacterDefense : MonoBehaviour
{
    [SerializeField] private CharacterDefendAnimation defendAnimation;
    [SerializeField] private CharacterInputReader inputReader;

    public bool IsDefending { get; private set; }

    private void Awake()
    {
        defendAnimation ??= GetComponent<CharacterDefendAnimation>();
        inputReader ??= GetComponent<CharacterInputReader>();
    }

    private void Update()
    {
        RefreshDefense();
    }

    public void SetUiDefense(bool value)
    {
        inputReader?.SetUiDefense(value);
        RefreshDefense();
    }

    private void RefreshDefense()
    {
        bool wantsDefense = inputReader != null && inputReader.WantsDefense;

        if (IsDefending == wantsDefense)
            return;

        IsDefending = wantsDefense;
        defendAnimation?.SetDefending(IsDefending);
    }
}