using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject cardsPanelRoot;
    [SerializeField] private GameObject quildPanelRoot;
    [SerializeField] private GameObject quildBacktoquildPanelRoot;
    [SerializeField] private GameObject otherPanelRoot;
    [SerializeField] private GameObject startGameWindowRoot;
    [SerializeField] private GameObject storyWindowRoot;
    [SerializeField] private GameObject hudWindowRoot;
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private GameObject deathScreenRoot;

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
    public bool HasWon { get; private set; } // il change
    private bool changedTimeScaleOnWin;
    
    private Health[] enemyHealthSources = new Health[0];
    private int initialEnemyCount;
    private int aliveEnemyCount;
    private Coroutine refreshEnemiesCoroutine;

    private void Awake()
    {
        playerHealth = mainCharacter != null ? mainCharacter.GetComponent<IDamageable>() : null;
        
        SetActiveIfAssigned(startGameWindowRoot, true);
        SetActiveIfAssigned(hudWindowRoot, false);
        SetActiveIfAssigned(menuRoot, false);
        SetActiveIfAssigned(cardsPanelRoot, false);
        SetActiveIfAssigned(quildPanelRoot, false);
        SetActiveIfAssigned(quildBacktoquildPanelRoot, false);
        SetActiveIfAssigned(otherPanelRoot, false);
        SetActiveIfAssigned(storyWindowRoot, false);
        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(deathScreenRoot, false);
    }

    private void Start()
    {
        InitializeScreens();
        
        timeScaleManager.Pause();
        isOpen = true;
    }

    private void InitializeScreens()
    {
        playerHealth.OnHealthChanged += OnPlayerHealthChanged;
        playerHealth.OnDeath += ShowDeathScreen;
        
        InitializeEnemies();
        refreshEnemiesCoroutine = StartCoroutine(RefreshEnemiesPeriodically());
    }
    
    private void OnDisable()
    {
        RestoreAllTimeScales();
        if (refreshEnemiesCoroutine != null)
        {
            StopCoroutine(refreshEnemiesCoroutine);
        }
        UnsubscribeFromDeaths();
        

            playerHealth.OnHealthChanged -= OnPlayerHealthChanged;
            playerHealth.OnDeath -= ShowDeathScreen;
        
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
            {
                return;
            }

            SetMenu(!isOpen);
        }
    }

    public void Resume()
    {
        isOpen = false;
        SetActiveIfAssigned(menuRoot, false);
        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(deathScreenRoot, false);
        SetActiveIfAssigned(hudWindowRoot, true);


            CanvasGroup hudCanvasGroup = hudWindowRoot.GetComponent<CanvasGroup>();
            if (hudCanvasGroup != null)
            {
                hudCanvasGroup.alpha = 1f;
                hudCanvasGroup.interactable = true;
                hudCanvasGroup.blocksRaycasts = true;
            }
        

        CloseAllPanels();
        timeScaleManager.Resume();
    }

    public void ContinueAndOpenQuitPopup()
    {
        Resume();
    }

    public void OpenMenu()
    {
        SetMenu(true);
    }

    public void CloseMenu()
    {
        SetMenu(false);
    }

    public void ToggleMenu()
    {
        SetMenu(!isOpen);
    }

    public bool IsOpen => isOpen;

    public void OpenQuitPopup()
    {
        if (!isOpen)
        {
            SetMenu(true);
        }
    }

    public void QuitGame()
    {
        timeScaleManager.Resume();
        Application.Quit();
    }

    public void QuitGameApplication()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenCardsPanel()
    {
        OpenPanel(cardsPanelRoot);
    }

    public void CloseCardsPanel()
    {
        SetPanelVisible(cardsPanelRoot, false);
    }

    public void OpenAdditionalPanel()
    {
        OpenPanel(quildPanelRoot);
    }

    public void CloseAdditionalPanel()
    {
        SetPanelVisible(quildPanelRoot, false);
    }

    public void OpenBacktoquildPanel()
    {
        OpenPanel(quildBacktoquildPanelRoot);
    }

    public void CloseBacktoquildPanel()
    {
        SetPanelVisible(quildBacktoquildPanelRoot, false);
    }

    public void OpenOtherPanel()
    {
        if (!isOpen)
        {
            SetMenu(true);
        }

        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(deathScreenRoot, false);
        SetPanelVisible(otherPanelRoot, true);
    }

    public void CloseOtherPanel()
    {
        SetPanelVisible(otherPanelRoot, false);
    }

    public void OpenStartGameWindow()
    {
        OpenPanel(startGameWindowRoot);
    }

    public void CloseStartGameWindow()
    {
        SetPanelVisible(startGameWindowRoot, false);
        isOpen = false;
        SetActiveIfAssigned(hudWindowRoot, true);
        timeScaleManager.Resume();
    }

    public void OpenStoryWindow()
    {
        OpenPanel(storyWindowRoot);
    }

    public void CloseStoryWindow()
    {
        SetPanelVisible(storyWindowRoot, false);
    }

    public void ShowDeathScreen()
    {
        if (deathScreenShown)
        {
            return;
        }

        deathScreenShown = true;
        SetActiveIfAssigned(deathScreenRoot, true);
        SetActiveIfAssigned(hudWindowRoot, false);

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
            changedTimeScaleOnDeath = true;
        }
    }

    private void OnPlayerHealthChanged(float currentHealth)
    {
        if (currentHealth <= 0f)
        {
            ShowDeathScreen();
        }
    }

    public void ShowWinScreen()
    {
        if (winScreenShown) return;

        winScreenShown = true;
        HasWon = true;
        SetActiveIfAssigned(winScreenRoot, true);
        SetActiveIfAssigned(hudWindowRoot, false);

        if (winScreenRoot != null)
        {
            CanvasGroup canvasGroup = winScreenRoot.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            Button[] buttons = winScreenRoot.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                btn.gameObject.SetActive(true);
            }
        }

        if (pauseGameOnWin)
        {
            Time.timeScale = 0f;
            changedTimeScaleOnWin = true;
        }
    }

    public void OnContinueButtonPressed()
    {
        Resume();
    }

    public void OnEndQuestButtonPressed()
    {
        if (endQuestSceneName != null && endQuestSceneName != "")
        {
            SceneManager.LoadScene(endQuestSceneName);
        }
    }

    private void TryShowWinScreen()
    {
        if (winScreenShown || playerHealth == null || playerHealth.CurrentHealth <= 0f || 
            initialEnemyCount <= 0 || aliveEnemyCount > 0)
        {
            return;
        }

        ShowWinScreen();
    }

    private void InitializeEnemies()
    {
        Health[] enemies = FindAllEnemies();
        initialEnemyCount = enemies.Length;
        aliveEnemyCount = CountAliveEnemies(enemies);
        SubscribeToDeaths(HandleEnemyDeath, enemies);
    }

    private Health[] FindAllEnemies()
    {
        Health[] allHealth = FindObjectsOfType<Health>();
        int enemyCount = 0;

        for (int i = 0; i < allHealth.Length; i++)
        {
            if (allHealth[i].Team == Team.Enemy)
            {
                enemyCount++;
            }
        }

        Health[] enemyHealth = new Health[enemyCount];
        int enemyIndex = 0;
        for (int i = 0; i < allHealth.Length; i++)
        {
            if (allHealth[i].Team == Team.Enemy)
            {
                enemyHealth[enemyIndex] = allHealth[i];
                enemyIndex++;
            }
        }

        enemyHealthSources = enemyHealth;
        return enemyHealth;
    }

    private int CountAliveEnemies(Health[] enemies = null)
    {
        Health[] checkEnemies = enemies ?? enemyHealthSources;
        int aliveCount = 0;
        for (int i = 0; i < checkEnemies.Length; i++)
        {
            if (checkEnemies[i] != null && checkEnemies[i].CurrentHealth > 0f)
            {
                aliveCount++;
            }
        }
        return aliveCount;
    }

    private void SubscribeToDeaths(System.Action onEnemyDeath, Health[] enemies = null)
    {
        Health[] subscribeEnemies = enemies ?? enemyHealthSources;
        for (int i = 0; i < subscribeEnemies.Length; i++)
        {
            if (subscribeEnemies[i] != null)
            {
                subscribeEnemies[i].OnDeath += onEnemyDeath;
            }
        }
    }

    private void UnsubscribeFromDeaths()
    {
        if (enemyHealthSources == null) return;

        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            if (enemyHealthSources[i] != null)
            {
                enemyHealthSources[i].OnDeath -= HandleEnemyDeath;
            }
        }
    }

    private void RefreshEnemyList()
    {
        Health[] currentEnemies = FindAllEnemies();
        
        if (currentEnemies.Length != initialEnemyCount)
        {
            UnsubscribeFromDeaths();
            initialEnemyCount = currentEnemies.Length;
            aliveEnemyCount = CountAliveEnemies();
            SubscribeToDeaths(HandleEnemyDeath);
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
        {
            return;
        }

        isOpen = open;
        if (menuRoot != null)
        {
            menuRoot.SetActive(open);
        }
        if (hudWindowRoot != null)
        {
            hudWindowRoot.SetActive(!open);
        }
        if (!open)
        {
            CloseAllPanels();
        }

        if (open)
            timeScaleManager.Pause();
        else
            timeScaleManager.Resume();
    }

    private bool IsEndScreenActive()
    {
        bool isWinScreenVisible = winScreenRoot != null && winScreenRoot.activeInHierarchy;
        bool isDeathScreenVisible = deathScreenRoot != null && deathScreenRoot.activeInHierarchy;
        return isWinScreenVisible || isDeathScreenVisible;
    }

    private void OpenPanel(GameObject panelRoot)
    {
        if (!isOpen)
        {
            SetMenu(true);
        }

        CloseAllPanels();
        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(deathScreenRoot, false);
        SetActiveIfAssigned(hudWindowRoot, false);
        SetPanelVisible(panelRoot, true);
    }

    private void CloseAllPanels()
    {
        SetPanelVisible(cardsPanelRoot, false);
        SetPanelVisible(quildPanelRoot, false);
        SetPanelVisible(quildBacktoquildPanelRoot, false);
        SetPanelVisible(otherPanelRoot, false);
        SetPanelVisible(storyWindowRoot, false);

        if (!isOpen && !IsAnyPanelOpen() && (startGameWindowRoot == null || !startGameWindowRoot.activeInHierarchy))
        {
            SetActiveIfAssigned(hudWindowRoot, true);
        }
    }

    private bool IsAnyPanelOpen()
    {
        return (cardsPanelRoot != null && cardsPanelRoot.activeInHierarchy) ||
               (quildPanelRoot != null && quildPanelRoot.activeInHierarchy) ||
               (quildBacktoquildPanelRoot != null && quildBacktoquildPanelRoot.activeInHierarchy) ||
               (otherPanelRoot != null && otherPanelRoot.activeInHierarchy) ||
               (storyWindowRoot != null && storyWindowRoot.activeInHierarchy);
    }

    private void SetPanelVisible(GameObject panelRoot, bool visible)
    {
        if (panelRoot == null)
        {
            return;
        }

        panelRoot.SetActive(visible);

        CanvasGroup canvasGroup = panelRoot.GetComponent<CanvasGroup>();
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
        {
            target.SetActive(isActive);
        }
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

