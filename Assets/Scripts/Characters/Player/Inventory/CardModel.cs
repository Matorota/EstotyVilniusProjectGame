using Configs;

namespace Characters.Player.Inventory
{
    public enum CardState
    {
        Collected,
        Equipped,
        Active
    }

    public class CardModel
    {
        public CardConfig config;
        public bool isEquipped;
        public bool isActive;
        public CardState state = CardState.Collected;

        public void MarkCollected()
        {
            isEquipped = false;
            isActive = false;
            state = CardState.Collected;
        }

        public void MarkEquipped()
        {
            isEquipped = true;
            isActive = false;
            state = CardState.Equipped;
        }

        public void MarkUnequipped()
        {
            isEquipped = false;
            isActive = false;
            state = CardState.Collected;
        }

        public void MarkActive()
        {
            if (!isEquipped)
            {
                return;
            }

            isActive = true;
            state = CardState.Active;
        }

        public void MarkInactive()
        {
            isActive = false;
            state = isEquipped ? CardState.Equipped : CardState.Collected;
        }
    }
}