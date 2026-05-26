using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHubWindow : MonoBehaviour
    {
        [SerializeField] private GameUIController gameUIController;
        [SerializeField] private Button gameGubMenuWindowButton;
        [SerializeField] private GameObject hudWindow; 
        [SerializeField] private CharacterMovements mainCharacter; 

        [SerializeField] private WinQuestWindow winQuestWindow;
        [SerializeField] private DeathWindow deathWindow;


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

        public void Open(bool forceShowHud = false)
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public bool IsOpen => gameObject.activeSelf;

    }
}