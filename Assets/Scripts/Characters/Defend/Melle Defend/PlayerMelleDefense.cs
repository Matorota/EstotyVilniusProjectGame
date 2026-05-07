using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelleDefense : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private CharacterDefendAnimation defendAnimation;
    [SerializeField] private CharacterInputReader inputReader;

    private bool uiHeld;
    private bool defending;

    private void Awake()
    {
        if (defendAnimation == null)
        {
            defendAnimation = GetComponent<CharacterDefendAnimation>();
        }
    }

    private void Update()
    {
        Vector2 movementInput = inputReader.ReadMovementInput();
        bool isMoving = movementInput.sqrMagnitude > 0.0001f;

        bool mouseHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;
        bool shouldDefend = !isMoving && (uiHeld || mouseHeld);
        SetDefending(shouldDefend);
    }

    public void OnDefendButtonDown() => uiHeld = true;
    public void OnDefendButtonUp() => uiHeld = false;

    private void SetDefending(bool value)
    {
        if (defending == value) return;
        defending = value;
        health.SetDefending(value);
        defendAnimation.SetDefending(value);
    }
}