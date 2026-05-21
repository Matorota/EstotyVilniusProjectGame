using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameStartWindow : MonoBehaviour
    {
        [SerializeField] private MenuController menuController;
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
            menuController.OpenStoryWindow();
            gameObject.SetActive(false);
        }
    }
}