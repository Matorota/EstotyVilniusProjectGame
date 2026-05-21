using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHubWindow : MonoBehaviour
    {
        [SerializeField] private GameUIController gameUIController;
        [SerializeField] private Button gameGubMenuWindowButton;

        private void OnEnable()
        {
            gameGubMenuWindowButton.onClick.AddListener(HandleMenuButtonClicked);
        }

        private void OnDisable()
        {
            gameGubMenuWindowButton.onClick.RemoveListener(HandleMenuButtonClicked);
        }
        
        private void HandleMenuButtonClicked()
        {
            gameUIController.OpenQuitPopup();
            gameObject.SetActive(false);
        }
    }
}