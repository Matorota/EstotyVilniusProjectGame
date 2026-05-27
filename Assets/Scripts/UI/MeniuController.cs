using UnityEngine;
using UnityEngine.InputSystem;

// There still the issue of FindWindow it is still there because of few windows being active and unactive when there not suppost to so this was for 
// a time being a short solution.

public class GameUIController : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private Transform respawnLocationCube;
    [SerializeField] private RespawnPlayer respawnPlayer;
    [SerializeField] private TimeScale timeScaleManager;

    private WinQuestWindow winQuestWindow;
    private DeathWindow deathWindowComponent;
    private MenuWindow menuWindow;
    private IDamageable playerHealth;
    private QuestRunner questRunner;
    private bool isDeathHandled;
    private bool isOpen;

    private void Awake()
    {
        playerHealth = mainCharacter?.GetComponent<IDamageable>();
    }

    private void Start()
    {
        ResolveWindowReferences();
        HideDeathScreen();
        ResetWinWindow();
        SetPlayerDeathState(false);
        timeScaleManager.Pause();
    }

    private void Update()
    {
        ResolveWindowReferences();
        RefreshPlayerHealthReference();
        HandleEndStates();

        if (IsEndScreenVisible())
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetMenu(!isOpen);
    }

    public void Resume()
    {
        isOpen = false;
        MenuWindow mw = menuWindow ?? FindWindow<MenuWindow>();
        if (mw != null)
            mw.Close();
        else if (timeScaleManager != null)
            timeScaleManager.Resume();
    }

    public void ContinueAndOpenQuitPopup() => Resume();

    public void OpenMenu() => SetMenu(true);

    public void CloseMenu() => SetMenu(false);

    public void ToggleMenu() => SetMenu(!isOpen);

    public bool IsOpen => isOpen;

    public bool IsQuestActive => questRunner != null && questRunner.Status == QuestStatus.Active;

    public void OpenQuitPopup()
    {
        if (!isOpen)
            SetMenu(true);
    }

    public void QuitGame()
    {
        timeScaleManager.Resume();
        Application.Quit();
    }

    public void QuitGameApplication()
    {
        Application.Quit();
    }

    public void RestartCurrentLevel()
    {

        if (!respawnPlayer.RespawnMainCharacter(mainCharacter, respawnLocationCube))
            return;

        isDeathHandled = false;
        HideDeathScreen();
        ResetWinWindow();
        SetPlayerDeathState(false);
        Time.timeScale = 1f;
        isOpen = false;
        timeScaleManager.Resume();
    }

    public void OpenCardsPanel() => SetMenu(true);

    public void CloseCardsPanel() => SetMenu(false);

    public void OpenAdditionalPanel() => SetMenu(true);

    public void CloseAdditionalPanel() => SetMenu(false);

    public void OpenBuildUI() => SetMenu(true);

    public void OpenOtherPanel() => SetMenu(true);

    public void CloseOtherPanel() => SetMenu(false);

    public void OnContinueButtonPressed() => Resume();

    private void SetMenu(bool open)
    {
        menuWindow = menuWindow ?? FindWindow<MenuWindow>();
        if (menuWindow != null)
        {
            if (open) menuWindow.Open(); else menuWindow.Close();
            isOpen = menuWindow.IsOpen;
            return;
        }

        // fallback when MenuWindow isn't present: manage timescale directly
        isOpen = open;
        if (open)
            timeScaleManager.Pause();
        else
            timeScaleManager.Resume();
    }

    private void ResolveWindowReferences()
    {
        if (winQuestWindow == null)
            winQuestWindow = FindWindow<WinQuestWindow>();

        if (deathWindowComponent == null)
            deathWindowComponent = FindWindow<DeathWindow>();

        if (menuWindow == null)
            menuWindow = FindWindow<MenuWindow>();

        if (questRunner == null)
            questRunner = FindWindow<QuestRunner>();
    }

    private void RefreshPlayerHealthReference()
    {
        if (playerHealth == null)
            playerHealth = mainCharacter?.GetComponent<IDamageable>();
    }

    private void HandleEndStates()
    {
        Health healthComp = GetPlayerHealth();
        if (healthComp == null)
            return;

        if (healthComp.CurrentHealth <= 0f)
        {
            if (!isDeathHandled)
            {
                isDeathHandled = true;
                SetPlayerDeathState(true);
                ShowDeathScreen();
            }

            return;
        }

        if (isDeathHandled)
        {
            isDeathHandled = false;
            HideDeathScreen();
            SetPlayerDeathState(false);
        }
    }

    private void ShowDeathScreen()
    {
        CloseMenu();

        if (deathWindowComponent != null)
        {
            deathWindowComponent.ShowWindow();
            return;
        }

        timeScaleManager.Pause();
    }

    private void HideDeathScreen()
    {
        if (deathWindowComponent != null)
        {
            deathWindowComponent.HideWindow();
        }
    }

    private bool IsEndScreenVisible()
    {
        if (winQuestWindow != null && winQuestWindow.IsVisible)
            return true;

        if (deathWindowComponent != null && deathWindowComponent.IsVisible)
            return true;

        return false;
    }

    private void ResetWinWindow()
    {
        if (winQuestWindow != null)
            winQuestWindow.HideWindow();
    }

    private T FindWindow<T>() where T : Component // was used for testing for now permanant (ask how to replace to what) I am for now thinking a registry or something similar
    {
        GameObject[] roots = gameObject.scene.GetRootGameObjects();
        for (int r = 0; r < roots.Length; r++)
        {
            T found = roots[r].GetComponentInChildren<T>(true);
            if (found != null)
                return found;
        }

        return null;
    }

    private void SetPlayerDeathState(bool isDead)
    {
        if (mainCharacter == null)
            return;

        CharacterMovements movements = mainCharacter.GetComponent<CharacterMovements>();
        if (movements != null)
            movements.enabled = !isDead;

        CharacterInputReader inputReader = mainCharacter.GetComponent<CharacterInputReader>();
        if (inputReader != null)
            inputReader.enabled = !isDead;

        CharacterMeleeAttack meleeAttack = mainCharacter.GetComponent<CharacterMeleeAttack>();
        if (meleeAttack != null)
            meleeAttack.enabled = !isDead;

        Combat combat = mainCharacter.GetComponent<Combat>();
        if (combat != null)
            combat.ClearTarget();

        CharacterMotor motor = mainCharacter.GetComponent<CharacterMotor>();
        if (motor != null)
            motor.ResetMotion();

        CharacterMovementAnimation movementAnimation = mainCharacter.GetComponent<CharacterMovementAnimation>();
        if (movementAnimation != null)
            movementAnimation.Tick(Vector2.zero, Vector3.zero, 0f, Vector3.zero);
    }

    public Health GetPlayerHealth() => playerHealth as Health ?? mainCharacter?.GetComponent<Health>();

    public GameObject[] GetSceneRoots() => gameObject.scene.GetRootGameObjects();

    public CharacterMovements GetMainCharacter() => mainCharacter;

}