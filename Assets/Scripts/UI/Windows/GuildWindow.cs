using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GuildWindow : MonoBehaviour
    {
        [SerializeField] private Transform questsContainer;
        [SerializeField] private QuestWidget questPrefab;
        [SerializeField] private List<QuestConfig> availableQuests = new List<QuestConfig>();
        [SerializeField] private Button openQuestButton;
        [SerializeField] private GameObject inventoryGuildSideWindowGameObject;
        [SerializeField] private QuestRunner questRunner;

        private void Awake()
        {
            questRunner ??= QuestRunner.Instance;

            if (questRunner != null)
                questRunner.OnQuestWon += HandleQuestWon;
        }

        private void OnDestroy()
        {
            if (questRunner != null)
                questRunner.OnQuestWon -= HandleQuestWon;
        }

        private void OnEnable()
        {
            openQuestButton.onClick.AddListener(HandleOpenQuestButtonClicked);
            Refresh();
        }

        private void OnDisable()
        {
            openQuestButton.onClick.RemoveListener(HandleOpenQuestButtonClicked);
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
            questRunner = QuestRunner.Instance;

        if (questRunner == null || questRunner.Status != QuestStatus.None)
            return;

        questRunner.StartQuest(quest);
        gameObject.SetActive(false);
    }
    
    private void HandleOpenQuestButtonClicked()
    {
        inventoryGuildSideWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void HandleQuestWon()
    {
        if (questRunner == null || questRunner.CurrentQuest == null)
            return;

        RemoveQuest(questRunner.CurrentQuest);
    }
}
}
