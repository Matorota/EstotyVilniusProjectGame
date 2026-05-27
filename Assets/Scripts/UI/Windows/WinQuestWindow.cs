using UnityEngine;
using UnityEngine.UI;

public class WinQuestWindow : MonoBehaviour
{
    [SerializeField] private QuestRunner questRunner;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private GameObject winScreenGameObject;
    [SerializeField] private Button buttonContinue;

    private CanvasGroup canvasGroup;
    private bool isVisible;
    private bool isInitialized;

    private void Awake()
    {
        EnsureInitialized();
        HideWindow();
    }

    private void OnDestroy()
    {
        if (!isInitialized)
            return;

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

        GameObject screenRoot = GetScreenRoot();
        if (!screenRoot.activeSelf)
            screenRoot.SetActive(true);
        screenRoot.transform.SetAsLastSibling();
        isVisible = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

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
        questRunner.EndQuest();
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

        ResolveQuestRunner();

        buttonContinue.onClick.AddListener(HandleContinueButtonClick);

        if (questRunner != null)
        {
            questRunner.OnQuestWon += HandleQuestWon;
            questRunner.OnQuestEnded += HandleQuestEnded;
        }

        isInitialized = true;
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

    private GameObject GetScreenRoot()
    {
        return winScreenGameObject != null ? winScreenGameObject : gameObject;
    }
}
