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
    [SerializeField] private UI.Windows.GameHubWindow hub;
    
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
        inventoryWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void HandleResumeButtonClick()
    {
        Close();
        hub.Open(true);
    }

    private void HandleQuitButtonClick()
    {
        Application.Quit();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        timeScaleManager.Pause();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        timeScaleManager.Resume();
    }

    public bool IsOpen => gameObject.activeSelf;
}
