using Configs;

namespace UI
{
    public class QuestRemoval
    {
        private GuildWindow _guildWindow;

        public QuestRemoval(GuildWindow guildWindow)
        {
            this._guildWindow = guildWindow;
        }

        public void FinishQuest(QuestConfig quest)
        {
            if (quest == null) return;

            if (_guildWindow != null)
            {
                _guildWindow.RemoveQuest(quest);
            }
        }
    }
}