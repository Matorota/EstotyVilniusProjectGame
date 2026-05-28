﻿using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class EndQuestWidget : MonoBehaviour
    {
        [SerializeField] private QuestRunner questRunner;
        [SerializeField] private Button endGameButton;
        [SerializeField] private GuildWindow guildWindow;
        [SerializeField] private PlayerLifecycle playerLifecycle;
        private CanvasGroup canvasGroup;


        private void OnEnable()
        {
            ResolveEndGameButton();
            if (endGameButton != null)
                endGameButton.onClick.AddListener(HandleEndGameButtonClick);
        }

        private void OnDisable()
        {
            if (endGameButton != null)
                endGameButton.onClick.RemoveListener(HandleEndGameButtonClick);
        }

        private void HandleEndGameButtonClick()
        {
            HideButton();

            if (questRunner != null)
                questRunner.EndQuest();

            if (guildWindow != null)
                guildWindow.gameObject.SetActive(true);
        }

        public void ShowButton()
        {
            ResolveEndGameButton();
            endGameButton.gameObject.SetActive(true);
            EnsureCanvasGroup();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            Transform parent = endGameButton.transform.parent;
            while (parent != null)
            {
                if (!parent.gameObject.activeSelf)
                    parent.gameObject.SetActive(true);

                CanvasGroup parentCG = parent.GetComponent<CanvasGroup>();
                if (parentCG != null)
                {
                    parentCG.alpha = 1f;
                    parentCG.interactable = true;
                    parentCG.blocksRaycasts = true;
                }
                parent = parent.parent;
            }
        }

        public void HideButton()
        {
            ResolveEndGameButton();
            EnsureCanvasGroup();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            endGameButton.gameObject.SetActive(false);
        }

        private void EnsureCanvasGroup()
        {
            if (canvasGroup == null && endGameButton != null)
            {
                canvasGroup = endGameButton.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = endGameButton.gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        private void ResolveEndGameButton()
        {
            if (endGameButton != null)
                return;
            endGameButton = GetComponentInChildren<Button>(true);
        }
    }
}