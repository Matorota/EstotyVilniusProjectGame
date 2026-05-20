using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private CharacterMovements mainCharacter;
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private GameObject hudWindowRoot;
    [SerializeField] private bool pauseGameOnWin = true;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private EndQuestButtonManager endQuestButtonManager;

    private IDamageable playerHealth;
    private CountEnemies countEnemies;
    private bool isShown;
    public bool HasWon;
    private bool changedTimeScale;
    private int initialEnemyCount;
    private int aliveEnemyCount;
    private float refreshInterval = 0.5f;
    private Coroutine refreshCoroutine;

    private void Awake()
    {
        playerHealth = mainCharacter != null ? mainCharacter.GetComponent<IDamageable>() : null;
        countEnemies = GetComponent<CountEnemies>() ?? gameObject.AddComponent<CountEnemies>();
        
        SetActiveIfAssigned(winScreenRoot, false);
        SetActiveIfAssigned(hudWindowRoot, true);

        if (playerHealth == null)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        InitializeEnemies();
        TryShowWinScreen();
    }

    private void OnEnable()
    {
        if (playerHealth == null) return;

        playerHealth.OnDeath += HandlePlayerDeath;
        InitializeEnemies();
        refreshCoroutine = StartCoroutine(RefreshEnemiesPeriodically());
    }


    private void RefreshEnemyList()
    {
        Health[] currentEnemies = countEnemies.FindAllEnemies();
        
        if (currentEnemies.Length != initialEnemyCount)
        {
            countEnemies.UnsubscribeFromDeaths(HandleEnemyDeath);
            initialEnemyCount = currentEnemies.Length;
            aliveEnemyCount = countEnemies.CountAliveEnemies();
            countEnemies.SubscribeToDeaths(HandleEnemyDeath);
            TryShowWinScreen();
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
        if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
            refreshCoroutine = null;
        }
        if (countEnemies != null)
        {
            countEnemies.UnsubscribeFromDeaths(HandleEnemyDeath);
        }
        RestoreTimeScaleIfChanged();
    }

    public void ShowWinScreen()
    {
        if (isShown) return;

        isShown = true;
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
            changedTimeScale = true;
        }
    }

    private void TryShowWinScreen()
    {
        if (isShown || playerHealth == null || playerHealth.CurrentHealth <= 0f || initialEnemyCount <= 0 || aliveEnemyCount > 0)
        {
            return;
        }

        ShowWinScreen();
    }

    private void InitializeEnemies()
    {
        Health[] enemies = countEnemies.FindAllEnemies();
        initialEnemyCount = enemies.Length;
        aliveEnemyCount = countEnemies.CountAliveEnemies();
        countEnemies.SubscribeToDeaths(HandleEnemyDeath);
    }

    private IEnumerator RefreshEnemiesPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshInterval);
            RefreshEnemyList();
        }
    }

    private void HandleEnemyDeath()
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        TryShowWinScreen();
    }

    private void HandlePlayerDeath()
    {
        enabled = false;
    }

    private void RestoreTimeScaleIfChanged()
    {
        if (!changedTimeScale) return;
        Time.timeScale = 1f;
        changedTimeScale = false;
    }

    private void SetActiveIfAssigned(GameObject target, bool isActive)
    {
        if (target != null) target.SetActive(isActive);
    }

    public void OnContinueButtonPressed()
    {
        if (pauseMenu != null)
        {
            pauseMenu.Resume();
        }
        
        if (endQuestButtonManager != null)
        {
            endQuestButtonManager.ShowEndQuestButton();
        }
    }
}
