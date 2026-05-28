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
            quitButton.onClick.AddListener(HandleQuitButtonClick);
        }

        private void OnDisable()
        {
            backButton.onClick.RemoveListener(HandleResumeButtonClick);
            quitButton.onClick.RemoveListener(HandleQuitButtonClick);
        }
        
        private void HandleResumeButtonClick()
        {
            if (menuWindow == null)
                return;

            menuWindow.Close();
        }

        private void HandleQuitButtonClick()
        {
            timeScaleManager?.Resume();
            Application.Quit();
        }

    }
}