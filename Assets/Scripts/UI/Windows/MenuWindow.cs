using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject inventoryWindowGameObject;
    [SerializeField] private TimeScale timeScaleManager;

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
        if (inventoryWindowGameObject != null)
            inventoryWindowGameObject.SetActive(true);
        
        gameObject.SetActive(false);
    }
    
    private void HandleResumeButtonClick()
    {
        if (timeScaleManager != null)
            timeScaleManager.Resume();
        
        gameObject.SetActive(false);
    }
    
    private void HandleQuitButtonClick()
    {
        Application.Quit();
    }
}
