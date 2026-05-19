using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "StoryConfig", menuName = "Configs/StoryConfig")]
    public class StoryConfig : ScriptableObject
    {
        [SerializeField] private string title;
        [TextArea(3,10)]
        [SerializeField] private string body;
        [SerializeField] private Sprite image;
        [SerializeField] private string speaker;

        public string Title => title;
        public string Body => body;
        public Sprite Image => image;
        public string Speaker => speaker;
    }
}
