using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private Transform respawnLocationCube;
    [SerializeField] private RespawnPlayer respawnPlayer;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private GuildWindow guildWindow;
    [SerializeField] private GameObject winWindow;
    [SerializeField] private GameObject deathWindow;
    [SerializeField] private GameObject menuRoot;

    [SerializeField] private bool pauseGameOnDeath = true;
    [SerializeField] private bool pauseGameOnWin = true;
    [SerializeField] private float enemyRefreshInterval = 0.5f;

    private bool isOpen;
    private IDamageable playerHealth;
    private Configs.QuestConfig currentQuest;

    private bool deathScreenShown;
    private bool changedTimeScaleOnDeath;

    private bool winScreenShown;
    public bool HasWon { get; private set; }
    private bool changedTimeScaleOnWin;

    private Health[] enemyHealthSources = new Health[0];
    private int initialEnemyCount;
    private int aliveEnemyCount;
    private Coroutine refreshEnemiesCoroutine;
    
    private void Awake()
    {
        playerHealth = mainCharacter?.GetComponent<IDamageable>();

        SetActiveIfAssigned(menuRoot, false);
        SetActiveIfAssigned(winWindow, false);
        SetActiveIfAssigned(deathWindow, false);
    }

    private void Start()
    {
        InitializeScreens();
        timeScaleManager.Pause();
    }

    private void InitializeScreens()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += OnPlayerHealthChanged;
            playerHealth.OnDeath += ShowDeathScreen;
        }

        InitializeEnemies();
        refreshEnemiesCoroutine = StartCoroutine(RefreshEnemiesPeriodically());
    }

    private void OnDisable()
    {
        RestoreAllTimeScales();

        if (refreshEnemiesCoroutine != null)
            StopCoroutine(refreshEnemiesCoroutine);

        UnsubscribeFromDeaths();

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnPlayerHealthChanged;
            playerHealth.OnDeath -= ShowDeathScreen;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (HasWon)
            {
                ShowWinScreen();
                return;
            }

            if (IsEndScreenActive())
                return;

            SetMenu(!isOpen);
        }
    }

    public void Resume()
    {
        isOpen = false;
        SetActiveIfAssigned(menuRoot, false);
        SetActiveIfAssigned(winWindow, false);
        SetActiveIfAssigned(deathWindow, false);
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

        Time.timeScale = 1f;
        deathScreenShown = false;
        winScreenShown = false;
        HasWon = false;
        changedTimeScaleOnDeath = false;
        changedTimeScaleOnWin = false;

        SetPanelVisible(deathWindow, false);
        SetPanelVisible(winWindow, false);
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

    public void ShowDeathScreen()
    {
        if (deathScreenShown) return;

        deathScreenShown = true;
        SetPanelVisible(deathWindow, true);

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
            changedTimeScaleOnDeath = true;
        }
    }

    private void OnPlayerHealthChanged(float currentHealth)
    {
        if (currentHealth <= 0f)
            ShowDeathScreen();
    }

    public void ShowWinScreen()
    {
        if (winScreenShown) return;

        winScreenShown = true;
        HasWon = true;
        
        SetPanelVisible(winWindow, true);

        if (winWindow != null)
        {
            var canvasGroup = winWindow.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            var buttons = winWindow.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
                btn.gameObject.SetActive(true);
        }

        if (pauseGameOnWin)
        {
            Time.timeScale = 0f;
            changedTimeScaleOnWin = true;
        }
    }

    public void OnContinueButtonPressed() => Resume();

    public void OnEndQuestButtonPressed()
    {
        Health healthComp = playerHealth as Health ?? mainCharacter?.GetComponent<Health>();
        if (healthComp != null)
        {
            float missing = healthComp.MaxHealth - healthComp.CurrentHealth;
            if (missing > 0f)
            {
                healthComp.Heal(missing);
            }
        }

        // Destroy all CardPickup instances in the active scene except the main character
        var roots = gameObject.scene.GetRootGameObjects(); // cannot think of another way how to do it without findobjectbytype
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
            {
                Destroy(cp.gameObject);
            }
        }

        var finishState = new UI.QuestRemoval(guildWindow);
        finishState.FinishQuest(currentQuest);

        OpenBuildUI();
    }

    private void TryShowWinScreen()
    {
        if (winScreenShown || playerHealth == null || playerHealth.CurrentHealth <= 0f ||
            initialEnemyCount <= 0 || aliveEnemyCount > 0)
            return;

        ShowWinScreen();
    }

    public void OnQuestStarted(Configs.QuestConfig quest)
    {
        currentQuest = quest;
        winScreenShown = false;
        HasWon = false;
        UnsubscribeFromDeaths();
        InitializeEnemies();
    }

    private void InitializeEnemies()
    {
        var enemies = FindAllEnemies();
        enemyHealthSources = enemies;
        initialEnemyCount = enemies.Length;
        aliveEnemyCount = CountAliveEnemies(enemies);
        SubscribeToDeaths(HandleEnemyDeath, enemies);
    }

    private Health[] FindAllEnemies()
    {
        var roots = gameObject.scene.GetRootGameObjects(); // cannot think of another way how to do it without findobjectbytype 
        var enemies = new System.Collections.Generic.List<Health>();
        for (int r = 0; r < roots.Length; r++)
        {
            var found = roots[r].GetComponentsInChildren<Health>(true);
            for (int i = 0; i < found.Length; i++)
            {
                var h = found[i];
                if (h != null && h.Team == Team.Enemy)
                    enemies.Add(h);
            }
        }

        return enemies.ToArray();
    }

    private int CountAliveEnemies(Health[] enemies = null)
    {
        var checkEnemies = enemies ?? enemyHealthSources;
        var aliveCount = 0;
        foreach (var e in checkEnemies)
        {
            if (e != null && e.CurrentHealth > 0f)
                aliveCount++;
        }

        return aliveCount;
    }

    private void SubscribeToDeaths(System.Action onEnemyDeath, Health[] enemies = null)
    {
        var subscribeEnemies = enemies ?? enemyHealthSources;
        foreach (var e in subscribeEnemies)
        {
            if (e != null)
                e.OnDeath += onEnemyDeath;
        }
    }

    private void UnsubscribeFromDeaths()
    {
        foreach (var e in enemyHealthSources)
        {
            if (e != null)
                e.OnDeath -= HandleEnemyDeath;
        }
    }

    private void RefreshEnemyList()
    {
        var currentEnemies = FindAllEnemies();
        if (currentEnemies.Length != initialEnemyCount)
        {
            UnsubscribeFromDeaths();
            enemyHealthSources = currentEnemies;
            initialEnemyCount = currentEnemies.Length;
            aliveEnemyCount = CountAliveEnemies(currentEnemies);
            SubscribeToDeaths(HandleEnemyDeath, currentEnemies);
            TryShowWinScreen();
        }
    }

    private IEnumerator RefreshEnemiesPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemyRefreshInterval);
            RefreshEnemyList();
        }
    }

    private void HandleEnemyDeath()
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        TryShowWinScreen();
    }

    private void SetMenu(bool open)
    {
        if (open && IsEndScreenActive())
            return;

        isOpen = open;
        SetActiveIfAssigned(menuRoot, open);

        if (open)
            timeScaleManager.Pause();
        else
            timeScaleManager.Resume();
    }

    private bool IsEndScreenActive()
    {
        return (winWindow != null && winWindow.activeInHierarchy) ||
               (deathWindow != null && deathWindow.activeInHierarchy);
    }

    private void SetPanelVisible(GameObject panelRoot, bool visible)
    {
        if (panelRoot == null) return;

        panelRoot.SetActive(visible);

        var canvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }

    private void SetActiveIfAssigned(GameObject target, bool isActive)
    {
        if (target != null)
            target.SetActive(isActive);
    }

    private void RestoreAllTimeScales()
    {
        if (changedTimeScaleOnDeath)
        {
            Time.timeScale = 1f;
            changedTimeScaleOnDeath = false;
        }

        if (changedTimeScaleOnWin)
        {
            Time.timeScale = 1f;
            changedTimeScaleOnWin = false;
        }
    }
}

public class MeniuController : GameUIController { }
