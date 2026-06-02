using System;
using Configs;
using UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHubWindow : MonoBehaviour
    {
        [SerializeField] private PauseWindow pauseWindow;
        [SerializeField] private Button gameGubMenuWindowButton;
        [SerializeField] private Button abilityDescriptionButton;
        [SerializeField] private AbilityWindow abilityWindow;
        [SerializeField] private QuestRunner questRunner;
        
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            EnsureCanvasGroup();

            if (questRunner != null)
            {
                questRunner.OnQuestStarted += HandleQuestStarted;
                questRunner.OnQuestEnded += HandleQuestEnded;
            }
        }

        private void OnDestroy()
        {
            if (questRunner != null)
            {
                questRunner.OnQuestStarted -= HandleQuestStarted;
                questRunner.OnQuestEnded -= HandleQuestEnded;
            }
        }

        private void OnEnable()
        {
            gameGubMenuWindowButton.onClick.AddListener(HandleMenuButtonClicked);
            if (abilityDescriptionButton != null)
                abilityDescriptionButton.onClick.AddListener(HandleAbilityDescriptionButtonClicked);
            else
                Debug.LogWarning("[GameHubWindow] abilityDescriptionButton is not assigned.", this);
        }

        private void OnDisable()
        {
            gameGubMenuWindowButton.onClick.RemoveListener(HandleMenuButtonClicked);
            if (abilityDescriptionButton != null)
                abilityDescriptionButton.onClick.RemoveListener(HandleAbilityDescriptionButtonClicked);
        }
        
        private void HandleMenuButtonClicked()
        {
            if (pauseWindow == null)
                return;

            pauseWindow.Open();
            Close();
        }

        private void HandleAbilityDescriptionButtonClicked()
        {
            ResolveAbilityWindow();

            if (abilityWindow == null)
            {
                Debug.LogWarning("[GameHubWindow] AbilityWindow is not assigned and could not be found in the scene.", this);
                return;
            }

            abilityWindow.Open();
            Close();
        }

        private void ResolveAbilityWindow()
        {
            if (abilityWindow != null)
                return;

            abilityWindow = FindFirstObjectByType<AbilityWindow>();
        }

        private void HandleQuestStarted(QuestConfig config)
        {
            Open();
        }

        private void HandleQuestEnded()
        {
            Close();
        }

        public void Open(bool forceShowHud = false)
        {
            gameObject.SetActive(true);
            EnsureCanvasGroup();
            UiVisibility.ShowWithParents(transform);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Close()
        {
            EnsureCanvasGroup();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public bool IsOpen => canvasGroup != null && canvasGroup.alpha > 0f;

        private void EnsureCanvasGroup()
        {
            if (canvasGroup != null)
                return;

            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

    }
}