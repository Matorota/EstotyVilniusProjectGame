﻿using System.Collections.Generic;
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
    [SerializeField] private UI.Windows.GameHubWindow hub;

    private Configs.QuestConfig currentQuest;

    public Configs.QuestConfig CurrentQuest => currentQuest;

    public QuestStatus Status { get; private set; } = QuestStatus.None;

    public bool IsActive => Status == QuestStatus.Active;

    public void StartQuest(Configs.QuestConfig quest)
    {
        currentQuest = quest;
        Status = QuestStatus.Active;
    }

    public void EndQuest()
    {
        currentQuest = null;
        Status = QuestStatus.None;
    }

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
        StartQuestInternal(quest);
    }

    private void StartQuestInternal(QuestConfig quest)
    {
        StartQuest(quest);

        timeScale.Resume();

        hudWindowGameObject.SetActive(true);

        hub.Open();

        enemySpawner.SpawnEnemies(quest.EnemiesAmount);

        levelProgressWidget.Setup(quest);

        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        openQuestButton.onClick.RemoveListener(HandleOpenQuestButtonClicked);
    }

    private void HandleOpenQuestButtonClicked()
    {
        inventoryGuildSideWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}

