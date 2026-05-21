using UnityEngine;
using UnityEngine.UI;

public class UIMenuWindow : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private Button backButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        backButton.onClick.AddListener(HandleResumeButtonClick);
        settingsButton.onClick.AddListener(HandleQuitButtonClick);
        quitButton.onClick.AddListener(HandleQuitButtonClick);
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(HandleResumeButtonClick);
        settingsButton.onClick.RemoveListener(HandleQuitButtonClick);
        quitButton.onClick.RemoveListener(HandleQuitButtonClick);
    }
    
    private void HandleResumeButtonClick()
    {
        gameUIController.CloseOtherPanel();
        gameObject.SetActive(false);
    }
    
    private void HandleQuitButtonClick() // for now for settings and quit
    {
        gameUIController.ContinueAndOpenQuitPopup();
        gameObject.SetActive(false);
    }
    
    
    
    
    
    
    
}