using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHubWindow : MonoBehaviour
    {
        [SerializeField] private GameUIController gameUIController;
        [SerializeField] private Button gameGubMenuWindowButton;
        [SerializeField] private GameObject hudWindow; // assign HudWindow root here
        [SerializeField] private CharacterMovements mainCharacter; // optional, auto-resolved if empty

        private WinQuestWindow winQuestWindow;
        private DeathWindow deathWindow;

        private void Awake()
        {
            if (gameUIController == null)
                gameUIController = FindObjectOfType<GameUIController>();

            if (mainCharacter == null && gameUIController != null)
                mainCharacter = gameUIController.GetMainCharacter();

            winQuestWindow = FindObjectOfType<WinQuestWindow>(true);
            deathWindow = FindObjectOfType<DeathWindow>(true);
        }

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

        private void Update()
        {
            if (hudWindow == null)
                return;

            bool questActive = gameUIController != null && gameUIController.IsQuestActive;
            bool menuOpen = gameUIController != null && gameUIController.IsOpen;
            bool winVisible = winQuestWindow != null && winQuestWindow.IsVisible;
            bool deathVisible = deathWindow != null && deathWindow.IsVisible;

            Health playerHealth = gameUIController != null ? gameUIController.GetPlayerHealth() : (mainCharacter != null ? mainCharacter.GetComponent<Health>() : null);
            bool playerDead = playerHealth != null && playerHealth.CurrentHealth <= 0f;

            bool blocking = menuOpen || winVisible || deathVisible || playerDead;

            // Show HUD when a quest is active and no blocking UI is open.
            bool shouldShow = questActive && !blocking;

            if (hudWindow.activeSelf != shouldShow)
                hudWindow.SetActive(shouldShow);
        }

    }
}