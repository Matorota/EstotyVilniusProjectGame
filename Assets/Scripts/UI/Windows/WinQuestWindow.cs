using UnityEngine;
using UnityEngine.UI;

public class WinQuestWindow : MonoBehaviour
{
    [SerializeField] private QuestRunner questRunner;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private GameObject winScreenGameObject;
    [SerializeField] private Button buttonContinue;
    [SerializeField] private GuildWindow guildWindow;

    private CanvasGroup canvasGroup;
    private bool isVisible;
    private bool isInitialized;
    private bool isQuestRunnerHooked = false;

    private void Awake()
    {
        EnsureInitialized();
        if (questRunner == null)
            questRunner = FindObjectOfType<QuestRunner>();
        if (questRunner != null && !isQuestRunnerHooked)
        {
            questRunner.OnQuestWon += HandleQuestWon;
            questRunner.OnQuestEnded += HandleQuestEnded;
            isQuestRunnerHooked = true;
        }

        HideWindow();
    }

    private void OnEnable()
    {
        EnsureInitialized();
        if (questRunner == null) questRunner = FindObjectOfType<QuestRunner>();
        if (questRunner != null && !isQuestRunnerHooked)
        {
            questRunner.OnQuestWon += HandleQuestWon;
            questRunner.OnQuestEnded += HandleQuestEnded;
            isQuestRunnerHooked = true;
        }
        if (buttonContinue != null)
            buttonContinue.onClick.AddListener(HandleContinueButtonClick);
    }

    private void OnDisable()
    {
        if (buttonContinue != null)
            buttonContinue.onClick.RemoveListener(HandleContinueButtonClick);

        if (questRunner != null && isQuestRunnerHooked)
        {
            questRunner.OnQuestWon -= HandleQuestWon;
            questRunner.OnQuestEnded -= HandleQuestEnded;
            isQuestRunnerHooked = false;
        }
    }

    private void OnDestroy()
    {
        if (buttonContinue != null)
            buttonContinue.onClick.RemoveListener(HandleContinueButtonClick);

        if (questRunner != null)
        {
            questRunner.OnQuestWon -= HandleQuestWon;
            questRunner.OnQuestEnded -= HandleQuestEnded;
        }
    }

    public void ShowWindow()
    {
        EnsureInitialized();

        if (buttonContinue != null)
        {
            buttonContinue.onClick.RemoveListener(HandleContinueButtonClick);
            buttonContinue.onClick.AddListener(HandleContinueButtonClick);
        }

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

        if (timeScaleManager != null)
            timeScaleManager.Pause();
        else
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
    }

    public bool IsVisible => isVisible;

    private void HandleContinueButtonClick()
    {
        questRunner?.EndQuest();
        HideWindow();
        
        if (guildWindow == null)
            guildWindow = ResolveGuildWindow();
        
        if (guildWindow != null)
            guildWindow.gameObject.SetActive(true);
    }
    
    private void HandleQuestWon()
    {
        ShowWindow();
    }

    private void HandleQuestEnded()
    {
        HideWindow();
    }

    private void EnsureInitialized()
    {
        if (isInitialized)
        {
            return;
        }

        GameObject screenRoot = GetScreenRoot();

        canvasGroup = screenRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = screenRoot.AddComponent<CanvasGroup>();

        isInitialized = true;
    }



    private GameObject GetScreenRoot()
    {
        return winScreenGameObject != null ? winScreenGameObject : gameObject;
    }

    private GuildWindow ResolveGuildWindow()
    {
        foreach (GuildWindow window in Resources.FindObjectsOfTypeAll<GuildWindow>())
        {
            if (window != null && window.gameObject.scene.IsValid())
                return window;
        }

        return null;
    }
}
