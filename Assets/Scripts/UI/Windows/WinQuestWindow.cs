using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinQuestWindow : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private GameObject winScreenGameObject;
    [SerializeField] private Button buttonContinue;
    [SerializeField] private GameObject hudWindowGameObject;
    [SerializeField] private Button endQuestButton;
    [SerializeField] private GuildWindow guildWindowRef;
    [SerializeField] private GameObject guildWindowGameObject;
    private GameObject menuUIGameObject;

    private CanvasGroup canvasGroup;
    private Health playerHealth;
    private readonly HashSet<Health> trackedEnemies = new HashSet<Health>();
    private bool isVisible;
    private bool isDismissed;
    private bool hasWon;
    private bool hasSeenLivingEnemy;
    private bool isInitialized;

    private void Awake()
    {
        EnsureInitialized();
        ResetState();
    }

    private void OnDestroy()
    {
        if (!isInitialized)
            return;

        buttonContinue.onClick.RemoveListener(HandleContinueButtonClick);
        endQuestButton.onClick.RemoveListener(HandleEndQuestButtonClick);

        EnemySpawner.OnEnemySpawned -= HandleEnemySpawned;
        ClearTrackedEnemies();
    }

    private void Update()
    {
        if (isVisible && gameUIController.IsOpen)
            gameUIController.CloseMenu();

        RefreshPlayerHealth();

        if (playerHealth == null || playerHealth.CurrentHealth <= 0f)
            return;

        EvaluateWinCondition();
    }

    public void ShowWindow()
    {
        EnsureInitialized();

        if (gameUIController.IsOpen)
            gameUIController.CloseMenu();

        GameObject screenRoot = GetScreenRoot();
        if (!screenRoot.activeSelf)
            screenRoot.SetActive(true);

        screenRoot.transform.SetAsLastSibling();
        isVisible = true;
        hasWon = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (timeScaleManager != null)
            timeScaleManager.Pause();
        else
            Time.timeScale = 0f;

        if (menuUIGameObject != null)
            menuUIGameObject.SetActive(false);

        if (endQuestButton != null)
            endQuestButton.gameObject.SetActive(false);
    }

    public void HideWindow()
    {
        EnsureInitialized();

        GameObject screenRoot = GetScreenRoot();
        isVisible = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (screenRoot != gameObject)
            screenRoot.SetActive(false);
    }

    public bool IsVisible => isVisible;

    public void ResetState()
    {
        EnsureInitialized();
        ClearTrackedEnemies();
        hasSeenLivingEnemy = false;
        isDismissed = false;
        hasWon = false;
        RegisterExistingEnemies();
        HideWindow();
    }

    private void RefreshPlayerHealth()
    {
        if (playerHealth == null)
            playerHealth = gameUIController.GetPlayerHealth();
    }

    private void EvaluateWinCondition()
    {
        if (isDismissed || hasWon)
            return;

        if (!hasSeenLivingEnemy)
            return;

        if (playerHealth == null || playerHealth.CurrentHealth <= 0f)
            return;

        if (CountAliveTrackedEnemies() == 0)
            ShowWindow();
    }

    private void RegisterExistingEnemies()
    {
        GameObject[] roots = gameUIController.GetSceneRoots();
        for (int r = 0; r < roots.Length; r++)
        {
            Health[] found = roots[r].GetComponentsInChildren<Health>(true);
            for (int i = 0; i < found.Length; i++)
                RegisterEnemy(found[i]);
        }
    }

    private void RegisterEnemy(Health health)
    {
        if (health == null || health.Team != Team.Enemy || trackedEnemies.Contains(health))
            return;

        trackedEnemies.Add(health);
        health.OnDeath += HandleTrackedEnemyDeath;

        if (health.CurrentHealth > 0f)
            hasSeenLivingEnemy = true;
    }

    private void ClearTrackedEnemies()
    {
        foreach (Health health in trackedEnemies)
        {
            if (health != null)
                health.OnDeath -= HandleTrackedEnemyDeath;
        }

        trackedEnemies.Clear();
    }

    private int CountAliveTrackedEnemies()
    {
        int aliveEnemyCount = 0;

        foreach (Health health in trackedEnemies)
        {
            if (health != null && health.gameObject.activeInHierarchy && health.CurrentHealth > 0f)
                aliveEnemyCount++;
        }

        return aliveEnemyCount;
    }

    private void HandleEnemySpawned(Health health)
    {
        RegisterEnemy(health);
        EvaluateWinCondition();
    }

    private void HandleTrackedEnemyDeath()
    {
        EvaluateWinCondition();
    }

    private void HandleContinueButtonClick()
    {
        isDismissed = true;

        if (timeScaleManager != null)
            timeScaleManager.Resume();
        else
            Time.timeScale = 1f;

        if (menuUIGameObject != null)
            menuUIGameObject.SetActive(true);

        if (endQuestButton != null)
            endQuestButton.gameObject.SetActive(true);

        HideWindow();
    }

    private void HandleEndQuestButtonClick()
    {
        ResetState();

        Health healthComp = gameUIController.GetPlayerHealth();
        if (healthComp != null)
        {
            float missing = healthComp.MaxHealth - healthComp.CurrentHealth;
            if (missing > 0f)
                healthComp.Heal(missing);
        }

        GameObject[] roots = gameUIController.GetSceneRoots();
        var cardsList = new System.Collections.Generic.List<CardPickup>();
        for (int r = 0; r < roots.Length; r++)
        {
            CardPickup[] found = roots[r].GetComponentsInChildren<CardPickup>(true);
            for (int j = 0; j < found.Length; j++)
                cardsList.Add(found[j]);
        }

        CharacterMovements mainCharacter = gameUIController.GetMainCharacter();
        for (int i = 0; i < cardsList.Count; i++)
        {
            CardPickup cp = cardsList[i];
            if (cp != null && cp.gameObject != mainCharacter?.gameObject)
                Destroy(cp.gameObject);
        }

        guildWindowRef.RemoveQuest(guildWindowRef.CurrentQuest);

        guildWindowRef.EndQuest();

        guildWindowGameObject.SetActive(true);

        HideWindow();
    }

    private void EnsureInitialized()
    {
        if (isInitialized)
            return;

        GameObject screenRoot = GetScreenRoot();

        canvasGroup = screenRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = screenRoot.AddComponent<CanvasGroup>();

        RefreshPlayerHealth();

        buttonContinue.onClick.AddListener(HandleContinueButtonClick);
        endQuestButton.onClick.AddListener(HandleEndQuestButtonClick);

        EnemySpawner.OnEnemySpawned += HandleEnemySpawned;
        isInitialized = true;
    }

    private GameObject GetScreenRoot()
    {
        return winScreenGameObject != null ? winScreenGameObject : gameObject;
    }
}
