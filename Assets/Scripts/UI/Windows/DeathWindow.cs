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
    [SerializeField] private Transform respawnLocationCube;
    [SerializeField] private TimeScale timeScaleManager;

    private CanvasGroup canvasGroup;
    private bool isVisible;
    private bool isInitialized;
    private QuestRunner questRunner;

    private void Awake()
    {
        EnsureInitialized();
        HideWindow();
    }

    private void OnEnable()
    {
        EnsureInitialized();
        ResolvePlayerLifecycle();
        SubscribeToPlayerLifecycle();

        restartButton?.onClick.AddListener(HandleRestartButtonClick);
        guildButton?.onClick.AddListener(HandleGuildButtonClick);
        quitButton?.onClick.AddListener(HandleQuitButtonClick);
    }

    private void OnDisable()
    {
        UnsubscribeFromPlayerLifecycle();
        restartButton?.onClick.RemoveListener(HandleRestartButtonClick);
        guildButton?.onClick.RemoveListener(HandleGuildButtonClick);
        quitButton?.onClick.RemoveListener(HandleQuitButtonClick);
    }

    private void OnDestroy()
    {
        // safety cleanup
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
        Time.timeScale = 0f;
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
        
        Time.timeScale = 1f;
    }

    public bool IsVisible => isVisible;

    private void HandleGuildButtonClick()
    {
        guildWindowGameObject.SetActive(true);
        HideWindow();
    }

    private void HandleRestartButtonClick()
    {
        ResolvePlayerLifecycle();
        ResolveQuestRunner();

        playerLifecycle?.Respawn(respawnLocationCube);
        questRunner?.EndQuest();
        timeScaleManager?.Resume();
        HideWindow();
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

        ResolvePlayerLifecycle();

        isInitialized = true;
    }

    private void ResolvePlayerLifecycle()
    {
        if (playerLifecycle != null)
            return;

        CharacterMovements charMovements = FindObjectOfType<CharacterMovements>();
        playerLifecycle = charMovements?.GetComponent<PlayerLifecycle>();

        if (playerLifecycle == null)
        {
            GameObject[] roots = gameObject.scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                playerLifecycle = root.GetComponentInChildren<PlayerLifecycle>(true);
                if (playerLifecycle != null)
                    return;
            }
        }
    }

    private void ResolveQuestRunner()
    {
        if (questRunner != null)
            return;

        GameObject[] roots = gameObject.scene.GetRootGameObjects();
        foreach (GameObject root in roots)
        {
            questRunner = root.GetComponentInChildren<QuestRunner>(true);
            if (questRunner != null)
                return;
        }
    }

    private void SubscribeToPlayerLifecycle()
    {
        if (playerLifecycle != null)
        {
            playerLifecycle.OnPlayerDied += HandlePlayerDied;
            playerLifecycle.OnPlayerRespawned += HandlePlayerRespawned;
        }
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

