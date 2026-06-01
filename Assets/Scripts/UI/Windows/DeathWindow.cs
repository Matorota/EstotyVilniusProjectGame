using Configs;
using UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class DeathWindow : MonoBehaviour
    {
    [SerializeField] private GameObject deathScreenGameObject;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button guildButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject guildWindowGameObject;
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

        if (questRunner != null)
        {
            questRunner.OnPlayerDied += HandlePlayerDied;
            questRunner.OnQuestStarted += HandleQuestStarted;
        }
    }

    private void OnEnable()
    {
        EnsureInitialized();

        restartButton?.onClick.AddListener(HandleRestartButtonClick);
        guildButton?.onClick.AddListener(HandleGuildButtonClick);
        quitButton?.onClick.AddListener(HandleQuitButtonClick);
    }

    private void OnDisable()
    {
        restartButton?.onClick.RemoveListener(HandleRestartButtonClick);
        guildButton?.onClick.RemoveListener(HandleGuildButtonClick);
        quitButton?.onClick.RemoveListener(HandleQuitButtonClick);
    }

    private void HandleQuestStarted(QuestConfig config)
    {
        HideWindow();
    }

    private void OnDestroy()
    {
        if (questRunner != null)
        {
            questRunner.OnPlayerDied -= HandlePlayerDied;
            questRunner.OnQuestStarted -= HandleQuestStarted;
        }

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

        UiVisibility.ShowWithParents(screenRoot.transform);

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


    private GameObject GetScreenRoot()
    {
        return deathScreenGameObject != null ? deathScreenGameObject : gameObject;
    }
}
}
