using System;
using Configs;
using UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class WinQuestWindow : MonoBehaviour
    {
    public static WinQuestWindow Instance { get; private set; }

    [SerializeField] private QuestRunner questRunner;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private Button buttonContinue;

    private CanvasGroup canvasGroup;
    private bool isVisible;

    private void Awake()
    {
        Instance = this;
        questRunner ??= QuestRunner.Instance;
        EnsureInitialized();
        HideWindow();

        if (questRunner != null)
        {
            questRunner.OnQuestWon += HandleQuestWon;
            questRunner.OnQuestEnded += HandleQuestEnded;
            questRunner.OnQuestStarted += HandleQuestStarted;
        }
    }

    private void OnDestroy()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestWon -= HandleQuestWon;
            questRunner.OnQuestEnded -= HandleQuestEnded;
            questRunner.OnQuestStarted -= HandleQuestStarted;
        }
    }

    private void OnEnable()
    {
        if (buttonContinue != null)
            buttonContinue.onClick.AddListener(HandleContinueButtonClick);
    }

    private void OnDisable()
    {
        if (buttonContinue != null)
            buttonContinue.onClick.RemoveListener(HandleContinueButtonClick);
    }

    private void HandleQuestWon()
    {
        ShowWindow();
    }

    private void HandleQuestEnded()
    {
        HideWindow();
    }

    public void ShowWindow()
    {
        if (questRunner == null || questRunner.Status != QuestStatus.Completed)
            return;

        EnsureInitialized();

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        UiVisibility.ShowWithParents(transform);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isVisible = true;

    }

    public void HideWindow()
    {
        EnsureInitialized();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        isVisible = false;
    }

    private void HandleQuestStarted(QuestConfig config)
    {
        HideWindow();
    }

    private void HandleContinueButtonClick()
    {
        HideWindow();
        questRunner?.ResetQuestState();

        if (EndQuestWidget.Instance != null)
            EndQuestWidget.Instance.ShowButton();
    }

    private void EnsureInitialized()
    {
        if (canvasGroup != null) return;

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
}
}