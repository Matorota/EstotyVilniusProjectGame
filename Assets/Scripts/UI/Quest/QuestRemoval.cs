using Configs;

namespace UI
{
    public class QuestRemoval
    {
        private QuestUiHolder questUiHolder;

        public QuestRemoval(QuestUiHolder questUiHolder)
        {
            this.questUiHolder = questUiHolder;
        }

        public void FinishQuest(QuestConfig quest)
        {
            if (quest == null) return;

            if (questUiHolder != null)
            {
                questUiHolder.RemoveQuest(quest);
            }
        }
    }
}