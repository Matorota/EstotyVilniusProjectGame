using Characters.Player.Inventory;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Windows
{
    public class AbilityWindow : MonoBehaviour
    {
        [SerializeField] private Transform abilitiesContainer;
        [SerializeField] private Widgets.AbilityDescriptionWidget abilityPrefab;
        [SerializeField] private SelectedCardsManager selectedCardsManager;
        [SerializeField] private Button resumeButton;
        [SerializeField] private GameHubWindow hub;
        [SerializeField] private TimeScale timeScaleManager;
        [SerializeField] private Abilities abilitiesConfig;
        [SerializeField] private TMP_Text emptyAbilitiesText;

        private void OnEnable()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(HandleResumeButtonClick);
            else
                Debug.LogWarning("[AbilityWindow] resumeButton is not assigned.", this);

            ResolveSelectedCardsManager();
            Refresh();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (gameObject.activeSelf)
                    HandleResumeButtonClick();
            }
        }

        private void OnDisable()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(HandleResumeButtonClick);

            if (selectedCardsManager != null)
                selectedCardsManager.OnSelectedChanged -= Refresh;
        }

        private void ResolveSelectedCardsManager()
        {
            SelectedCardsManager found = FindFirstObjectByType<SelectedCardsManager>();
            if (found == selectedCardsManager)
                return;

            if (selectedCardsManager != null)
                selectedCardsManager.OnSelectedChanged -= Refresh;
            selectedCardsManager = found;
            if (selectedCardsManager != null)
                selectedCardsManager.OnSelectedChanged += Refresh;
        }

        private void Refresh()
        {
            ResolveSelectedCardsManager();

            if (abilitiesContainer == null)
            {
                Debug.LogWarning("[AbilityWindow] abilitiesContainer is not assigned.", this);
                return;
            }

            if (abilityPrefab == null)
            {
                Debug.LogWarning("[AbilityWindow] abilityPrefab is not assigned.", this);
                return;
            }

            for (int i = abilitiesContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = abilitiesContainer.GetChild(i);
                if (emptyAbilitiesText != null && child == emptyAbilitiesText.transform)
                    continue;

                Destroy(child.gameObject);
            }

            if (selectedCardsManager == null)
            {
                Debug.LogWarning("[AbilityWindow] SelectedCardsManager not assigned.", this);
                return;
            }

            var equippedCards = selectedCardsManager.GetSelectedCards();
            bool hasAny = false;

            foreach (var card in equippedCards)
            {
                if (card.Config == null)
                    continue;

                Sprite icon = abilitiesConfig != null
                    ? abilitiesConfig.GetImage(card.Type)
                    : card.Config.Image;

                var widget = Instantiate(abilityPrefab, abilitiesContainer);
                widget.Bind(icon, card.Config.Name, card.Config.Description);
                hasAny = true;
            }

            if (emptyAbilitiesText != null)
                emptyAbilitiesText.gameObject.SetActive(!hasAny);
        }

        private void HandleResumeButtonClick()
        {
            Close();
            hub?.Open(true);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            timeScaleManager?.Pause();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            timeScaleManager?.Resume();
        }

        public bool IsOpen => gameObject.activeSelf;
    }
}

