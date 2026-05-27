using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.UI;

public class GuildWindow : MonoBehaviour
{
    [SerializeField] private Transform questsContainer;
    [SerializeField] private QuestWidget questPrefab;
    [SerializeField] private List<QuestConfig> availableQuests = new List<QuestConfig>();
    [SerializeField] private Button openQuestButton;
    [SerializeField] private GameObject inventoryGuildSideWindowGameObject;
    [SerializeField] private QuestRunner questRunner;

    private bool isQuestRunnerHooked;

    private void Awake()
    {
        ResolveQuestRunner();
        HookQuestRunner();
    }

    private void OnEnable()
    {
        ResolveQuestRunner();
        HookQuestRunner();
        openQuestButton.onClick.AddListener(HandleOpenQuestButtonClicked);
        Refresh();

    }

    public void Refresh()
    {
        for (int i = questsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(questsContainer.GetChild(i).gameObject);
        }

        if (availableQuests.Count == 0)
        {
            return;
        }

        for (int i = 0; i < availableQuests.Count; i++)
        {
            QuestConfig quest = availableQuests[i];
            QuestWidget widget = Instantiate(questPrefab, questsContainer);
            widget.gameObject.name = $"Quest_{quest.Name}_{i}";
            widget.Bind(quest, HandleQuestClicked);
        }
    }

    public void RemoveQuest(QuestConfig quest)
    {
        if (availableQuests.Contains(quest))
        {
            availableQuests.Remove(quest);
            Refresh();
        }
    }

    private void HandleQuestClicked(QuestConfig quest)
    {
        if (questRunner == null)
        {
            ResolveQuestRunner();
        }

        if (questRunner == null)
        {
            Debug.LogError("GuildWindow requires a QuestRunner to start quests.");
            return;
        }

        if (questRunner.Status != QuestStatus.None)
        {
            Debug.LogWarning("GuildWindow tried to start a quest while another quest is already in progress.");
            return;
        }

        questRunner.StartQuest(quest);
        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        openQuestButton.onClick.RemoveListener(HandleOpenQuestButtonClicked);
    }

    private void OnDestroy()
    {
        UnhookQuestRunner();
    }

    private void HandleOpenQuestButtonClicked()
    {
        inventoryGuildSideWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void HandleQuestEnded()
    {
        if (questRunner == null || questRunner.CurrentQuest == null)
        {
            return;
        }

        RemoveQuest(questRunner.CurrentQuest);
    }

    private void ResolveQuestRunner()
    {
        if (questRunner != null)
        {
            return;
        }

        GameObject[] roots = gameObject.scene.GetRootGameObjects();
        for (int r = 0; r < roots.Length; r++)
        {
            questRunner = roots[r].GetComponentInChildren<QuestRunner>(true);
            if (questRunner != null)
            {
                return;
            }
        }
    }

    private void HookQuestRunner()
    {
        if (isQuestRunnerHooked || questRunner == null)
        {
            return;
        }

        questRunner.OnQuestEnded += HandleQuestEnded;
        isQuestRunnerHooked = true;
    }

    private void UnhookQuestRunner()
    {
        if (!isQuestRunnerHooked || questRunner == null)
        {
            return;
        }

        questRunner.OnQuestEnded -= HandleQuestEnded;
        isQuestRunnerHooked = false;
    }
}
