﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class EndQuestWidget : MonoBehaviour
    {
        [SerializeField] private GameUIController gameUIController;
        [SerializeField] private Button endGameButton;


        private void OnEnable()
        {
            endGameButton.onClick.AddListener(HandleEndGameButtonClick);

        }

        private void OnDisable()
        {
            endGameButton.onClick.RemoveListener(HandleEndGameButtonClick);

        }

        private void HandleEndGameButtonClick()
        {
            if (endGameButton != null)
            {
                endGameButton.gameObject.SetActive(false);
            }

            if (gameUIController != null)
            {
                gameUIController.OnEndQuestButtonPressed();
            }
        }
    

    }
}