using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (questPrefab == null || availableQuests == null)
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
        if (quest == null || string.IsNullOrWhiteSpace(quest.TargetSceneName))
        {
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(quest.TargetSceneName);
    }
}
