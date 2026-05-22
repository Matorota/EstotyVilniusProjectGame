using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.UI;

public class QuestUiHolder : MonoBehaviour
{
    [SerializeField] private Transform questsContainer;
    [SerializeField] private QuestWidget questPrefab;
    [SerializeField] private List<QuestConfig> availableQuests = new List<QuestConfig>();
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Widgets.LevelProgressWidget levelProgressWidget;
    [SerializeField] private Button openQuestButton;
    
    private void OnEnable()
    {
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
        gameUIController.OnQuestStarted(quest);
        gameUIController.Resume();
        enemySpawner.SpawnEnemies(quest.EnemiesAmount);
        levelProgressWidget.Setup(quest);
    }
    
    private void OnDisable()
    {
        openQuestButton.onClick.RemoveListener(HandleOpenQuestButtonClicked);
    }

    private void HandleOpenQuestButtonClicked()
    {
        gameUIController.OpenBacktoquildPanel();
        gameObject.SetActive(false);
    }
}

