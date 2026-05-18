using Configs;

namespace Characters.Player.Inventory
{
    public class CardModel
    {
        public CardConfig config;
        public bool isEquipped;
        public bool isActive;

        public void MarkCollected()
        {
            isEquipped = false;
            isActive = false;
        }

        public void MarkEquipped()
        {
            isEquipped = true;
            isActive = false;
        }

        public void MarkUnequipped()
        {
            isEquipped = false;
            isActive = false;
        }

        public void MarkActive()
        {
            if (!isEquipped)
            {
                return;
            }

            isActive = true;
        }

        public void MarkInactive()
        {
            isActive = false;
        }
    }
}