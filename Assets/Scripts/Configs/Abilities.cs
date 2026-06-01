using Characters.Player.Inventory;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "AbilitiesConfig", menuName = "Configs/AbilitiesConfig")]
    public class Abilities : ScriptableObject
    {
        [SerializeField] private Sprite speedUpImage;
        [SerializeField] private Sprite damageImage;
        [SerializeField] private Sprite healthImage;

        public Sprite GetImage(CardType type)
        {
            return type switch
            {
                CardType.SpeedUp => speedUpImage,
                CardType.Damage => damageImage,
                CardType.Health => healthImage,
                _ => null
            };
        }
    }
}