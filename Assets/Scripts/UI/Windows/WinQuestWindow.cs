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
        Debug.Log($"[WinQuestWindow] Awake. questRunner={(questRunner != null ? "found" : "null")}", this);
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
        Debug.Log("[WinQuestWindow] Subscribed to QuestRunner events.", this);
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
        Debug.Log("[WinQuestWindow] HandleQuestWon called!", this);
        ShowWindow();
    }

    private void HandleQuestEnded()
    {
        HideWindow();
    }

    public void ShowWindow()
    {
        if (questRunner == null || questRunner.Status != QuestStatus.Completed)
        {
            Debug.Log($"[WinQuestWindow] ShowWindow blocked. Status={questRunner?.Status}", this);
            return;
        }

        Debug.Log($"[WinQuestWindow] ShowWindow called. gameObject.activeSelf={gameObject.activeSelf} canvasGroup={(canvasGroup != null ? "ok" : "NULL")}", this);

        EnsureInitialized();

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        EnsureParentsActive();

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isVisible = true;

        timeScaleManager?.Pause();

        Debug.Log("[WinQuestWindow] ShowWindow finished. Window should be visible.", this);
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
        ResumeTime();

        if (EndQuestWidget.Instance != null)
            EndQuestWidget.Instance.ShowButton();
    }

    private void ResumeTime()
    {
        timeScaleManager?.Resume();
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