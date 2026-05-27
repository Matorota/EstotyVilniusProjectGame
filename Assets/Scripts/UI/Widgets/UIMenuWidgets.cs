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
            ResolveMenuWindow();
            menuWindow?.Close();
        }

        private void HandleSettingsButtonClick()
        {
            
        }
        
        private void HandleQuitButtonClick()
        {
            timeScaleManager?.Resume();
            Application.Quit();
        }

        private void ResolveMenuWindow()
        {
            if (menuWindow != null)
                return;

            GameObject[] roots = gameObject.scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                menuWindow = root.GetComponentInChildren<MenuWindow>(true);
                if (menuWindow != null)
                    return;
            }
        }
    }
}