using Configs;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGuildSideWindow : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button openGuildButton;
    [SerializeField] private Button startQuestButton;
    [SerializeField] private GameObject inventoryMenuWindowGameObject;
    [SerializeField] private GameObject guildWindowGameObject;
    [SerializeField] private SelectedAbilitiesUi selectedAbilitiesUi;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private QuestRunner questRunner;
    [SerializeField] private QuestWidget questDisplayWidget;
    [SerializeField] private GameHubWindow hub;

    private QuestConfig selectedQuest;

    private void OnEnable()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestStarted += HandleQuestStarted;
        }

        timeScaleManager?.Pause();
        menuButton.onClick.AddListener(HandleMenuButtonClick);
        openGuildButton.onClick.AddListener(HandleOpenGuildButtonClicked);
        if (startQuestButton != null)
            startQuestButton.onClick.AddListener(HandleStartQuestButtonClicked);
        selectedAbilitiesUi?.gameObject.SetActive(true);
        RefreshQuestDisplay();
    }

    private void OnDisable()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestStarted -= HandleQuestStarted;
        }

        menuButton.onClick.RemoveListener(HandleMenuButtonClick);
        openGuildButton.onClick.RemoveListener(HandleOpenGuildButtonClicked);
        if (startQuestButton != null)
            startQuestButton.onClick.RemoveListener(HandleStartQuestButtonClicked);
        selectedAbilitiesUi?.gameObject.SetActive(false);
        timeScaleManager?.Resume();
    }

    private void HandleQuestStarted(Configs.QuestConfig config)
    {
        gameObject.SetActive(false);
        timeScaleManager?.Resume();
    }

    private void HandleMenuButtonClick()
    {
        timeScaleManager?.Pause();
        inventoryMenuWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void HandleOpenGuildButtonClicked()
    {
        selectedQuest = null;
        guildWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Setup(QuestConfig quest)
    {
        selectedQuest = quest;
    }

    private void RefreshQuestDisplay()
    {
        if (questDisplayWidget == null)
        {
            Debug.LogWarning("[InventoryGuildSideWindow] questDisplayWidget is not assigned.", this);
            return;
        }

        questDisplayWidget.Setup(selectedQuest);
    }

    private void HandleStartQuestButtonClicked()
    {
        if (questRunner == null || selectedQuest == null)
            return;

        questRunner.StartQuest(selectedQuest);
        hub?.Open(true);
    }
}