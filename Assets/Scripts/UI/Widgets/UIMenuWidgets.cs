using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class UIMenuWidgets : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TimeScale timeScaleManager;
        [SerializeField] private MenuWindow menuWindow;

        private void OnEnable()
        {
            backButton.onClick.AddListener(HandleResumeButtonClick);
            settingsButton.onClick.AddListener(HandleSettingsButtonClick);
            quitButton.onClick.AddListener(HandleQuitButtonClick);
        }

        private void OnDisable()
        {
            backButton.onClick.RemoveListener(HandleResumeButtonClick);
            settingsButton.onClick.RemoveListener(HandleSettingsButtonClick);
            quitButton.onClick.RemoveListener(HandleQuitButtonClick);
        }
        
        private void HandleResumeButtonClick()
        {
            if (menuWindow == null)
            {
                Debug.LogError("UIMenuWidgets: menuWindow not assigned in Inspector.");
                return;
            }

            menuWindow.Close();
        }

        private void HandleSettingsButtonClick()
        {
            
        }
        
        private void HandleQuitButtonClick()
        {
            timeScaleManager?.Resume();
            Application.Quit();
        }

    }
}