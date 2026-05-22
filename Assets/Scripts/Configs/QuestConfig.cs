using Characters.Player.Inventory;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "QuestConfig", menuName = "Configs/QuestConfig")]
    public class QuestConfig : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private Sprite image;
        [SerializeField] private int enemiesAmount = 5;
        [SerializeField] private string isCompletedText = "Completed!";
        [SerializeField] private bool isCompleted  = false;
        

        public string Name => name;
        public string Description => description;
        public Sprite Image => image;
        public int EnemiesAmount => enemiesAmount;
        public bool QuestState => isCompleted;
        public string IsCompletedText => isCompletedText;

    }
}
