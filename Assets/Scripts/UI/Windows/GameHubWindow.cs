using System;
using Configs;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHubWindow : MonoBehaviour
    {
        public static GameHubWindow Instance { get; private set; }
        [SerializeField] private MenuWindow menuWindow;
        [SerializeField] private Button gameGubMenuWindowButton;
        [SerializeField] private QuestRunner questRunner;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            Instance = this;
            questRunner ??= QuestRunner.Instance;
            SubscribeToQuestRunner();
            EnsureCanvasGroup();
        }

        private void OnEnable()
        {
            gameGubMenuWindowButton.onClick.AddListener(HandleMenuButtonClicked);
        }

        private void OnDisable()
        {
            gameGubMenuWindowButton.onClick.RemoveListener(HandleMenuButtonClicked);
        }

        private void SubscribeToQuestRunner()
        {
            if (questRunner == null)
                return;

            questRunner.OnQuestStarted -= HandleQuestStarted;
            questRunner.OnQuestStarted += HandleQuestStarted;
            questRunner.OnQuestEnded -= HandleQuestEnded;
            questRunner.OnQuestEnded += HandleQuestEnded;
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
            EnsureCanvasGroup();
            EnsureParentsActive();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            Debug.Log("[GameHubWindow] Open called. HUD should be visible.", this);
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

        private void EnsureParentsActive()
        {
            Transform current = transform.parent;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                    current.gameObject.SetActive(true);

                CanvasGroup parentCG = current.GetComponent<CanvasGroup>();
                if (parentCG != null)
                {
                    parentCG.alpha = 1f;
                    parentCG.interactable = true;
                    parentCG.blocksRaycasts = true;
                }

                current = current.parent;
            }
        }

    }
}