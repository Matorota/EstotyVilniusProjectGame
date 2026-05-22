using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameStartWindow : MonoBehaviour
    {
        [SerializeField] private GameUIController gameUIController;
        [SerializeField] private Button startGameButton;

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
            gameUIController.OpenStoryWindow();
            gameObject.SetActive(false);
        }
    }
}