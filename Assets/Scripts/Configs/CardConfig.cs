using Characters.Player.Inventory;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Configs/CardConfig")]
    public class CardConfig : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private Sprite image;
        [SerializeField] private CardType type;
        [SerializeField] private float value;
        [SerializeField] private float duration = 5f;
        
        public string Name => name;
        public string Description => description;
        public Sprite Image => image;
        public CardType Type => type;
        public float Value => value;
        public float Duration => duration;
    }
}