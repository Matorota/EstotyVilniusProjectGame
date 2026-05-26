using Configs;

public static class ActiveQuestRegistry // need it to get CurrentQuest from anywhere
{
    public static QuestConfig CurrentQuest { get; set; }

    public static bool HasActiveQuest => CurrentQuest != null;
}