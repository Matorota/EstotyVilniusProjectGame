using UnityEngine;
using UnityEngine.InputSystem;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private Transform respawnLocationCube;
    [SerializeField] private RespawnPlayer respawnPlayer;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private GuildWindow guildWindow;
    [SerializeField] private GameObject menuRoot;

    private bool isOpen;
    private IDamageable playerHealth;
    private Configs.QuestConfig currentQuest;
    private WinQuestWindow winQuestWindow;
    private DeathWindow deathWindowComponent;
    private bool isDeathHandled;

    private void Awake()
    {
        playerHealth = mainCharacter?.GetComponent<IDamageable>();
        SetActiveIfAssigned(menuRoot, false);
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
        SetActiveIfAssigned(menuRoot, false);
        timeScaleManager.Resume();
    }

    public void ContinueAndOpenQuitPopup() => Resume();

    public void OpenMenu() => SetMenu(true);

    public void CloseMenu() => SetMenu(false);

    public void ToggleMenu() => SetMenu(!isOpen);

    public bool IsOpen => isOpen;

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
        if (respawnPlayer == null)
        {
            Debug.LogError("Respawn player is not assigned.");
            return;
        }

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

    public void OnEndQuestButtonPressed()
    {
        Health healthComp = playerHealth as Health ?? mainCharacter?.GetComponent<Health>();
        if (healthComp != null)
        {
            float missing = healthComp.MaxHealth - healthComp.CurrentHealth;
            if (missing > 0f)
                healthComp.Heal(missing);
        }

        var roots = gameObject.scene.GetRootGameObjects();
        var cardsList = new System.Collections.Generic.List<CardPickup>();
        for (int r = 0; r < roots.Length; r++)
        {
            var found = roots[r].GetComponentsInChildren<CardPickup>(true);
            for (int j = 0; j < found.Length; j++)
                cardsList.Add(found[j]);
        }

        for (int i = 0; i < cardsList.Count; i++)
        {
            var cp = cardsList[i];
            if (cp != null && cp.gameObject != mainCharacter?.gameObject)
                Destroy(cp.gameObject);
        }

        var finishState = new UI.QuestRemoval(guildWindow);
        finishState.FinishQuest(currentQuest);

        OpenBuildUI();
    }

    public void OnQuestStarted(Configs.QuestConfig quest)
    {
        currentQuest = quest;
        isDeathHandled = false;
        HideDeathScreen();
        ResetWinWindow();
        SetPlayerDeathState(false);
    }

    private void SetMenu(bool open)
    {
        isOpen = open;
        SetActiveIfAssigned(menuRoot, open);

        if (open)
            timeScaleManager.Pause();
        else
            timeScaleManager.Resume();
    }

    private void SetActiveIfAssigned(GameObject target, bool isActive)
    {
        if (target != null)
            target.SetActive(isActive);
    }

    private void ResolveWindowReferences()
    {
        if (winQuestWindow == null)
            winQuestWindow = FindWindow<WinQuestWindow>();

        if (deathWindowComponent == null)
            deathWindowComponent = FindWindow<DeathWindow>();
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
            winQuestWindow.ResetState();
    }

    private T FindWindow<T>() where T : Component
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

    public void FinishQuest()
    {
        if (currentQuest != null && guildWindow != null)
            guildWindow.RemoveQuest(currentQuest);
    }
}

public class MeniuController : GameUIController { }
