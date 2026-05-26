using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class StartWindow : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private StoryWindow storyWindow;

        private void Awake()
        {
            storyWindow.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            startGameButton.onClick.AddListener(HandleStartButtonClicked);
        }

        private void OnDisable()
        {
            startGameButton.onClick.RemoveListener(HandleStartButtonClicked);
        }

        private void HandleStartButtonClicked()
        {
            storyWindow.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
