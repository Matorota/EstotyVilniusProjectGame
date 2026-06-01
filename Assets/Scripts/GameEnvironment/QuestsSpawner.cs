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

        if (questRunner != null)
        {
            questRunner.OnQuestStarted += HandleQuestStarted;
            questRunner.OnQuestEnded += HandleQuestEnded;
        }

        HideAll();
    }

    private void OnDestroy()
    {
        if (questRunner != null)
        {
            questRunner.OnQuestStarted -= HandleQuestStarted;
            questRunner.OnQuestEnded -= HandleQuestEnded;
        }
    }

    private void HandleQuestStarted(QuestConfig quest)
    {
        if (quest == null || questRunner == null)
            return;

        RefreshForQuest(quest);
    }

    private void HandleQuestEnded()
    {
        HideAll();
    }

    private void RefreshForQuest(QuestConfig quest)
    {
        HashSet<GameObject> toActivate = new();

        foreach (QuestWorldGroup group in worldGroups)
        {
            if (group.quest == quest && group.worldObjects != null)
            {
                foreach (GameObject obj in group.worldObjects)
                {
                    if (obj != null)
                        toActivate.Add(obj);
                }
            }
        }

        foreach (QuestWorldGroup group in worldGroups)
        {
            if (group.worldObjects == null)
                continue;

            foreach (GameObject obj in group.worldObjects)
            {
                if (obj == null)
                    continue;

                obj.SetActive(toActivate.Contains(obj));
            }
        }
    }

    private void HideAll()
    {
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
