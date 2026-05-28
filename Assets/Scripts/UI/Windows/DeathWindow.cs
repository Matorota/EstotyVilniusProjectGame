using Configs;
using UnityEngine;
using UnityEngine.UI;

public class DeathWindow : MonoBehaviour
{
    [SerializeField] private GameObject deathScreenGameObject;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button guildButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject guildWindowGameObject;
    [SerializeField] private PlayerLifecycle playerLifecycle;
    [SerializeField] private QuestRunner questRunner;
    [SerializeField] private Transform respawnLocationCube;
    [SerializeField] private TimeScale timeScaleManager;

    private CanvasGroup canvasGroup;
    private bool isVisible;
    private bool isInitialized;

    private void Awake()
    {
        EnsureInitialized();
        HideWindow();
    }

    private void OnEnable()
    {
        EnsureInitialized();
        ResolvePlayerLifecycle();

        if (questRunner == null)
            questRunner = QuestRunner.Instance;
        if (questRunner != null)
            questRunner.OnQuestStarted += HandleQuestStarted;

        restartButton?.onClick.AddListener(HandleRestartButtonClick);
        guildButton?.onClick.AddListener(HandleGuildButtonClick);
        quitButton?.onClick.AddListener(HandleQuitButtonClick);
    }

    private void OnDisable()
    {
        UnsubscribeFromPlayerLifecycle();

        if (questRunner != null)
            questRunner.OnQuestStarted -= HandleQuestStarted;

        restartButton?.onClick.RemoveListener(HandleRestartButtonClick);
        guildButton?.onClick.RemoveListener(HandleGuildButtonClick);
        quitButton?.onClick.RemoveListener(HandleQuitButtonClick);
    }

    private void HandleQuestStarted(QuestConfig config)
    {
        ResolvePlayerLifecycle();
    }

    private void ResolvePlayerLifecycle()
    {
        // Always unsubscribe from old to prevent double-subscription or stale references
        if (playerLifecycle != null)
        {
            playerLifecycle.OnPlayerDied -= HandlePlayerDied;
            playerLifecycle.OnPlayerRespawned -= HandlePlayerRespawned;
        }

        playerLifecycle = PlayerLifecycle.Instance;
        if (playerLifecycle != null)
        {
            playerLifecycle.OnPlayerDied += HandlePlayerDied;
            playerLifecycle.OnPlayerRespawned += HandlePlayerRespawned;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromPlayerLifecycle();
        restartButton?.onClick.RemoveListener(HandleRestartButtonClick);
        guildButton?.onClick.RemoveListener(HandleGuildButtonClick);
        quitButton?.onClick.RemoveListener(HandleQuitButtonClick);
    }

    public void ShowWindow()
    {
        EnsureInitialized();

        GameObject screenRoot = GetScreenRoot();
        if (!screenRoot.activeSelf)
            screenRoot.SetActive(true);

        screenRoot.transform.SetAsLastSibling();
        isVisible = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Transform parent = screenRoot.transform.parent;
        while (parent != null)
        {
            if (!parent.gameObject.activeSelf)
                parent.gameObject.SetActive(true);

            CanvasGroup parentCG = parent.GetComponent<CanvasGroup>();
            if (parentCG != null)
            {
                parentCG.alpha = 1f;
                parentCG.interactable = true;
                parentCG.blocksRaycasts = true;
            }
            parent = parent.parent;
        }

        timeScaleManager?.Pause();
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
        
        timeScaleManager?.Resume();
    }

    public bool IsVisible => isVisible;

    private void HandleGuildButtonClick()
    {
        questRunner?.EndQuest();
        timeScaleManager?.Resume();
        HideWindow();
        guildWindowGameObject?.SetActive(true);
    }

    private void HandleRestartButtonClick()
    {
        QuestConfig questToRestart = questRunner?.CurrentQuest;
        CardInventory.ResetInventory();
        timeScaleManager?.Resume();
        HideWindow();

        if (questToRestart != null)
            questRunner?.StartQuest(questToRestart);
    }

    private void HandleQuitButtonClick()
    {
        timeScaleManager?.Resume();
        Application.Quit();
    }

    private void HandlePlayerDied()
    {
        ShowWindow();
    }

    private void HandlePlayerRespawned()
    {
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

        isInitialized = true;
    }


    private void UnsubscribeFromPlayerLifecycle()
    {
        if (playerLifecycle != null)
        {
            playerLifecycle.OnPlayerDied -= HandlePlayerDied;
            playerLifecycle.OnPlayerRespawned -= HandlePlayerRespawned;
        }
    }

    private GameObject GetScreenRoot()
    {
        return deathScreenGameObject != null ? deathScreenGameObject : gameObject;
    }
}

