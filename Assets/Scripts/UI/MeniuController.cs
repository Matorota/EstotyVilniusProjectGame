using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private GameObject quildWindow;
    [SerializeField] private GameObject inventoryFromQuildSide;
    [SerializeField] private GameObject inventoryMenuWindow;
    [SerializeField] private GameObject startGameWindow;
    [SerializeField] private GameObject storyWindow;
    [SerializeField] private GameObject hudWindow;
    [SerializeField] private GameObject winWindow;
    [SerializeField] private GameObject deathWindow;

    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private TimeScaleManager timeScaleManager;

    [SerializeField] private bool pauseGameOnDeath = true;
    [SerializeField] private bool pauseGameOnWin = true;
    [SerializeField] private float enemyRefreshInterval = 0.5f;
    [SerializeField] private string endQuestSceneName = "MainMenu";

    private bool isOpen;
    private IDamageable playerHealth;

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

        SetActiveIfAssigned(startGameWindow, true);
        SetActiveIfAssigned(hudWindow, false);
        SetActiveIfAssigned(menuRoot, false);
        SetActiveIfAssigned(inventoryWindow, false);
        SetActiveIfAssigned(quildWindow, false);
        SetActiveIfAssigned(inventoryFromQuildSide, false);
        SetActiveIfAssigned(inventoryMenuWindow, false);
        SetActiveIfAssigned(storyWindow, false);
        SetActiveIfAssigned(winWindow, false);
        SetActiveIfAssigned(deathWindow, false);
    }

    private void Start()
    {
        InitializeScreens();
        timeScaleManager.Pause();
        isOpen = true;
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
        SetPanelVisible(hudWindow, true);

        CloseAllPanels();
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenCardsPanel() => OpenPanel(inventoryWindow);

    public void CloseCardsPanel() => SetPanelVisible(inventoryWindow, false);

    public void OpenAdditionalPanel() => OpenPanel(quildWindow);

    public void CloseAdditionalPanel() => SetPanelVisible(quildWindow, false);

    public void OpenBacktoquildPanel() => OpenPanel(inventoryFromQuildSide);

    public void CloseBacktoquildPanel() => SetPanelVisible(inventoryFromQuildSide, false);

    public void OpenOtherPanel()
    {
        if (!isOpen)
            SetMenu(true);

        SetActiveIfAssigned(winWindow, false);
        SetActiveIfAssigned(deathWindow, false);
        SetPanelVisible(inventoryMenuWindow, true);
    }

    public void CloseOtherPanel() => SetPanelVisible(inventoryMenuWindow, false);

    public void OpenStartGameWindow() => OpenPanel(startGameWindow);

    public void CloseStartGameWindow()
    {
        SetPanelVisible(startGameWindow, false);
        isOpen = false;
        SetPanelVisible(hudWindow, true);
        timeScaleManager.Resume();
    }

    public void OpenStoryWindow() => OpenPanel(storyWindow);

    public void CloseStoryWindow() => SetPanelVisible(storyWindow, false);

    public void ShowDeathScreen()
    {
        if (deathScreenShown) return;

        deathScreenShown = true;
        SetPanelVisible(deathWindow, true);
        SetActiveIfAssigned(hudWindow, false);

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
        SetActiveIfAssigned(hudWindow, false);

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
        // Reset main character health to max if possible
        Health healthComp = playerHealth as Health ?? mainCharacter?.GetComponent<Health>();
        if (healthComp != null)
        {
            float missing = healthComp.MaxHealth - healthComp.CurrentHealth;
            if (missing > 0f)
            {
                healthComp.Heal(missing);
            }
        }

        // Remove all spawned card pickups from the world
        CardDropManager.ClearAll();

        if (!string.IsNullOrEmpty(endQuestSceneName))
            SceneManager.LoadScene(endQuestSceneName);
    }

    private void TryShowWinScreen()
    {
        if (winScreenShown || playerHealth == null || playerHealth.CurrentHealth <= 0f ||
            initialEnemyCount <= 0 || aliveEnemyCount > 0)
            return;

        ShowWinScreen();
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
        var allHealth = FindObjectsOfType<Health>();
        var enemies = new List<Health>(allHealth.Length);
        foreach (var h in allHealth)
        {
            if (h != null && h.Team == Team.Enemy)
                enemies.Add(h);
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
        SetActiveIfAssigned(hudWindow, !open);

        if (!open)
            CloseAllPanels();

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

    private void OpenPanel(GameObject panelRoot)
    {
        if (!isOpen)
            SetMenu(true);

        CloseAllPanels();
        SetActiveIfAssigned(winWindow, false);
        SetActiveIfAssigned(deathWindow, false);
        SetActiveIfAssigned(hudWindow, false);
        SetPanelVisible(panelRoot, true);
    }

    private void CloseAllPanels()
    {
        SetPanelVisible(inventoryWindow, false);
        SetPanelVisible(quildWindow, false);
        SetPanelVisible(inventoryFromQuildSide, false);
        SetPanelVisible(inventoryMenuWindow, false);
        SetPanelVisible(storyWindow, false);

        if (!isOpen && !IsAnyPanelOpen() && (startGameWindow == null || !startGameWindow.activeInHierarchy))
            SetActiveIfAssigned(hudWindow, true);
    }

    private bool IsAnyPanelOpen()
    {
        return (inventoryWindow != null && inventoryWindow.activeInHierarchy) ||
               (quildWindow != null && quildWindow.activeInHierarchy) ||
               (inventoryFromQuildSide != null && inventoryFromQuildSide.activeInHierarchy) ||
               (inventoryMenuWindow != null && inventoryMenuWindow.activeInHierarchy) ||
               (storyWindow != null && storyWindow.activeInHierarchy);
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

