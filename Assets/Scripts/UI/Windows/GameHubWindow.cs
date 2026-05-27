using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHubWindow : MonoBehaviour
    {
        [SerializeField] private MenuWindow menuWindow;
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
            ResolveMenuWindow();
            menuWindow?.Open();
            gameObject.SetActive(false);
        }

        public void Open(bool forceShowHud = false)
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public bool IsOpen => gameObject.activeSelf;

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