using System;
using Configs;
using UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHubWindow : MonoBehaviour
    {
        [SerializeField] private MenuWindow menuWindow;
        [SerializeField] private Button gameGubMenuWindowButton;
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
        }

        private void OnDisable()
        {
            gameGubMenuWindowButton.onClick.RemoveListener(HandleMenuButtonClicked);
        }
        
        private void HandleMenuButtonClicked()
        {
            if (menuWindow == null)
                return;

            menuWindow.Open();
            Close();
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