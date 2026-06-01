using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;

public class QuestsSpawner : MonoBehaviour
{
    [Serializable]
    public class QuestWorldGroup
    {
        public QuestConfig quest;
        public GameObject[] worldObjects;
    }

    [SerializeField] private QuestRunner questRunner;
    [SerializeField] private List<QuestWorldGroup> worldGroups = new();

    private void Awake()
    {
        questRunner ??= QuestRunner.Instance;
        HideAll();
    }

    private void OnEnable()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestStarted += HandleQuestStarted;
            questRunner.OnQuestEnded += HandleQuestEnded;

            Debug.Log($"[QuestsSpawner] OnEnable. CurrentQuest={questRunner.CurrentQuest?.name}, Status={questRunner.Status}");

            if (questRunner.CurrentQuest != null && questRunner.Status == QuestStatus.Active)
                RefreshForQuest(questRunner.CurrentQuest);
            else
                HideAll();
        }
        else
        {
            Debug.LogWarning("[QuestsSpawner] OnEnable: questRunner is null!");
        }
    }

    private void OnDisable()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestStarted -= HandleQuestStarted;
            questRunner.OnQuestEnded -= HandleQuestEnded;
        }
    }

    private void HandleQuestStarted(QuestConfig quest)
    {
        Debug.Log($"[QuestsSpawner] HandleQuestStarted called for quest: {quest?.name}");

        if (quest == null)
        {
            Debug.LogWarning("[QuestsSpawner] HandleQuestStarted: quest is null!");
            return;
        }

        RefreshForQuest(quest);
    }

    private void HandleQuestEnded()
    {
        Debug.Log("[QuestsSpawner] HandleQuestEnded called.");
        HideAll();
    }

    private void RefreshForQuest(QuestConfig quest)
    {
        Debug.Log($"[QuestsSpawner] RefreshForQuest: {quest.name}. WorldGroups count: {worldGroups.Count}");

        HashSet<GameObject> toActivate = new();

        foreach (QuestWorldGroup group in worldGroups)
        {
            Debug.Log($"[QuestsSpawner] Checking group: quest={group.quest?.name}, matches={group.quest == quest}");
            if (group.quest == quest && group.worldObjects != null)
            {
                foreach (GameObject obj in group.worldObjects)
                {
                    if (obj != null)
                    {
                        toActivate.Add(obj);
                        Debug.Log($"[QuestsSpawner] Added to activate: {obj.name}");
                    }
                }
            }
        }

        Debug.Log($"[QuestsSpawner] Total objects to activate: {toActivate.Count}");

        foreach (QuestWorldGroup group in worldGroups)
        {
            if (group.worldObjects == null)
                continue;

            foreach (GameObject obj in group.worldObjects)
            {
                if (obj == null)
                    continue;

                bool shouldShow = toActivate.Contains(obj);
                Debug.Log($"[QuestsSpawner] Setting {obj.name} active = {shouldShow}");
                obj.SetActive(shouldShow);
            }
        }
    }

    private void HideAll()
    {
        Debug.Log("[QuestsSpawner] HideAll called.");
        foreach (QuestWorldGroup group in worldGroups)
        {
            if (group.worldObjects == null)
                continue;

            foreach (GameObject obj in group.worldObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}
