using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GuildWindow : MonoBehaviour
{
    [SerializeField] private Transform questsContainer;
    [SerializeField] private QuestWidget questPrefab;
    [SerializeField] private List<QuestConfig> availableQuests = new List<QuestConfig>();
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Widgets.LevelProgressWidget levelProgressWidget;
    [SerializeField] private Button openQuestButton;
    [SerializeField] private GameObject inventoryGuildSideWindowGameObject;
    [SerializeField] private GameObject hudWindowGameObject;
    
    [SerializeField] private TimeScale timeScale;

    private Configs.QuestConfig currentQuest;
    
    public Configs.QuestConfig CurrentQuest => currentQuest;

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
        // Start quest locally: set active quest, resume time, show HUD, spawn enemies, setup UI.
        currentQuest = quest;
        ActiveQuestRegistry.CurrentQuest = quest;

        if (timeScale != null)
            timeScale.Resume();

        if (hudWindowGameObject != null)
            hudWindowGameObject.SetActive(true);

        if (enemySpawner != null)
            enemySpawner.SpawnEnemies(quest.EnemiesAmount);

        if (levelProgressWidget != null)
            levelProgressWidget.Setup(quest);

        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        openQuestButton.onClick.RemoveListener(HandleOpenQuestButtonClicked);
    }

    private void HandleOpenQuestButtonClicked()
    {
        if (inventoryGuildSideWindowGameObject != null)
            inventoryGuildSideWindowGameObject.SetActive(true);
        
        gameObject.SetActive(false);
    }
}

