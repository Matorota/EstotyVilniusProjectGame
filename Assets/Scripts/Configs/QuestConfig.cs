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
       // [SerializeField] private int gameCompletion;
        [SerializeField] private string targetSceneName;

        
        public string Name => name;
        public string Description => description;
        public Sprite Image => image;
        //public int GameCompletion => gameCompletion;
        public string TargetSceneName => targetSceneName;

    }
}
