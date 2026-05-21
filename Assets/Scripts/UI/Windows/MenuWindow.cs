using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        inventoryButton.onClick.AddListener(HandleInventoryButtonClick);
        resumeButton.onClick.AddListener(HandleResumeButtonClick);
        settingsButton.onClick.AddListener(HandleQuitButtonClick);
        quitButton.onClick.AddListener(HandleQuitButtonClick);
    }

    private void OnDisable()
    {
        inventoryButton.onClick.RemoveListener(HandleInventoryButtonClick);
        resumeButton.onClick.RemoveListener(HandleResumeButtonClick);
        settingsButton.onClick.RemoveListener(HandleQuitButtonClick);
        quitButton.onClick.RemoveListener(HandleQuitButtonClick);
    }

    private void HandleInventoryButtonClick()
    {
        gameUIController.OpenCardsPanel();
        gameObject.SetActive(false);
    }
    
    private void HandleResumeButtonClick()
    {
        gameUIController.Resume();
        gameObject.SetActive(false);
    }
    
    private void HandleQuitButtonClick() // for now for settings and quit
    {
        gameUIController.ContinueAndOpenQuitPopup();
        gameObject.SetActive(false);
    }
}
