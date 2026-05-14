using Characters.Player.Inventory;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Configs/CardConfig")]
    public class CardConfig : ScriptableObject
    {
        [SerializeField] private string cardId;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private Sprite image;
        [SerializeField] private CardType type;
        [SerializeField] private int amount;
        
        public string CardId => cardId;
        public string Name => name;
        public string Description => description;
        public Sprite Image => image;
        public CardType Type => type;
        public int Amount => amount;
    }
}