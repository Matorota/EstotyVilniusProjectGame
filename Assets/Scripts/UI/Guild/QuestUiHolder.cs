using System.Collections.Generic;
using Configs;
using UnityEngine;

public class QuestUiHolder : MonoBehaviour
{
    [SerializeField] private Transform questsContainer;
    [SerializeField] private QuestWidget questPrefab;
    [SerializeField] private List<QuestConfig> availableQuests = new List<QuestConfig>();

    private void Awake()
    {
        questsContainer ??= transform;
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (questsContainer == null)
        {
            return;
        }

        for (int i = questsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(questsContainer.GetChild(i).gameObject);
        }

        if (questPrefab == null)
        {
            return;
        }

        if (availableQuests.Count == 0)
        {
            return;
        }

        for (int i = 0; i < availableQuests.Count; i++)
        {
            QuestConfig quest = availableQuests[i];
            if (quest == null)
            {
                continue;
            }

            QuestWidget widget = Instantiate(questPrefab, questsContainer);
            widget.gameObject.name = $"Quest_{quest.Name}_{i}";
            widget.Bind(quest, HandleQuestClicked);
        }
    }

    private void HandleQuestClicked(QuestConfig quest)
    {
        if (quest == null)
        {
            return;
        }

        // Close UI and start gameplay
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu != null)
        {
            pauseMenu.Resume(); // This closes all panels and shows HUD
        }

        // Spawn enemies based on quest config
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.SpawnEnemies(quest.EnemiesAmount);
        }
    }
}
