using UnityEngine;
using UnityEngine.UI;

public class InventoryGuildSideWindow : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button openGuildButton;
    [SerializeField] private GameObject inventoryMenuWindowGameObject;
    [SerializeField] private GameObject guildWindowGameObject;
    [SerializeField] private SelectedAbilitiesUi selectedAbilitiesUi;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private QuestRunner questRunner;

    private void OnEnable()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestStarted += HandleQuestStarted;
        }

        timeScaleManager?.Pause();
        menuButton.onClick.AddListener(HandleMenuButtonClick);
        openGuildButton.onClick.AddListener(HandleOpenGuildButtonClicked);
        ResolveSelectedAbilitiesUi();
        selectedAbilitiesUi?.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestStarted -= HandleQuestStarted;
        }

        menuButton.onClick.RemoveListener(HandleMenuButtonClick);
        openGuildButton.onClick.RemoveListener(HandleOpenGuildButtonClicked);
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
        guildWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ResolveSelectedAbilitiesUi()
    {
        if (selectedAbilitiesUi != null)
            return;

        selectedAbilitiesUi = GetComponentInChildren<SelectedAbilitiesUi>(true);
        if (selectedAbilitiesUi != null)
            return;

        GameObject[] roots = gameObject.scene.GetRootGameObjects();
        foreach (GameObject root in roots)
        {
            selectedAbilitiesUi = root.GetComponentInChildren<SelectedAbilitiesUi>(true);
            if (selectedAbilitiesUi != null)
                return;
        }
    }
}