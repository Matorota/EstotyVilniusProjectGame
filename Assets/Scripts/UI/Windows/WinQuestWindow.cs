using System;
using Configs;
using UnityEngine;
using UnityEngine.UI;
using UI.Windows;

public class WinQuestWindow : MonoBehaviour
{
    public static WinQuestWindow Instance { get; private set; }

    [SerializeField] private QuestRunner questRunner;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private Button buttonContinue;

    private CanvasGroup canvasGroup;
    private bool isVisible;
    private bool isSubscribed;

    private void Awake()
    {
        Instance = this;
        questRunner ??= QuestRunner.Instance;
        EnsureInitialized();
        HideWindow();
    }
    private void OnEnable()
    {
        if (questRunner == null)
            questRunner = QuestRunner.Instance;

        SubscribeEvents();

        if (buttonContinue != null)
            buttonContinue.onClick.AddListener(HandleContinueButtonClick);
    }

    private void Start()
    {
        if (questRunner == null)
            questRunner = QuestRunner.Instance;

        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();

        if (buttonContinue != null)
            buttonContinue.onClick.RemoveListener(HandleContinueButtonClick);
    }

    private void SubscribeEvents()
    {
        if (questRunner == null || isSubscribed)
            return;

        questRunner.OnQuestWon += HandleQuestWon;
        questRunner.OnQuestEnded += HandleQuestEnded;
        questRunner.OnQuestStarted += HandleQuestStarted;
        isSubscribed = true;
    }

    private void UnsubscribeEvents()
    {
        if (questRunner == null || !isSubscribed)
            return;

        questRunner.OnQuestWon -= HandleQuestWon;
        questRunner.OnQuestEnded -= HandleQuestEnded;
        questRunner.OnQuestStarted -= HandleQuestStarted;
        isSubscribed = false;
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

        EnsureParentsActive();

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

    private void EnsureParentsActive()
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (!current.gameObject.activeSelf)
                current.gameObject.SetActive(true);

            CanvasGroup parentCG = current.GetComponent<CanvasGroup>();
            if (parentCG != null)
            {
                parentCG.alpha = 1f;
                parentCG.interactable = true;
                parentCG.blocksRaycasts = true;
            }

            current = current.parent;
        }
    }

}