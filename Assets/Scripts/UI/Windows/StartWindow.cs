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
            if (storyWindow != null)
                storyWindow.gameObject.SetActive(false);

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            if (startGameButton != null)
                startGameButton.onClick.AddListener(HandleStartButtonClicked);
        }

        private void OnDisable()
        {
            if (startGameButton != null)
                startGameButton.onClick.RemoveListener(HandleStartButtonClicked);
        }

        private void HandleStartButtonClicked()
        {
            storyWindow.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
