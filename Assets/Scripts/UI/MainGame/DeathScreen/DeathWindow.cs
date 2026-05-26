﻿using UnityEngine;
using UnityEngine.UI;

public class DeathWindow : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quildButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject guildWindowGameObject;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(HandleRestartButtonClick);
        quildButton.onClick.AddListener(HandleQuildButtonClick);
        quitButton.onClick.AddListener(HandleQuitButtonClick);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(HandleRestartButtonClick);
        quildButton.onClick.RemoveListener(HandleQuildButtonClick);
        quitButton.onClick.RemoveListener(HandleQuitButtonClick);
    }

    private void HandleQuildButtonClick()
    {
        if (guildWindowGameObject != null)
            guildWindowGameObject.SetActive(true);
        
        gameObject.SetActive(false);
    }
    
    private void HandleRestartButtonClick()
    {
        gameUIController.RestartCurrentLevel();
        gameObject.SetActive(false);
    }
    
    private void HandleQuitButtonClick() // for now for settings and quit
    {
        gameUIController.ContinueAndOpenQuitPopup();
        gameObject.SetActive(false);
    }
}