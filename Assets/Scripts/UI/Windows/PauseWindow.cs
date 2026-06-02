using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseWindow : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private GameObject inventoryWindowGameObject;
    [SerializeField] private TimeScale timeScaleManager;
    [SerializeField] private UI.Windows.GameHubWindow hub;

    private void OnEnable()
    {
        resumeButton.onClick.AddListener(HandleResumeButtonClick);

    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveListener(HandleResumeButtonClick);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (gameObject.activeSelf)
                Close();
            else
                Open();
        }
    }

    private void HandleInventoryButtonClick()
    {
        inventoryWindowGameObject.SetActive(true);
        Close();
    }

    private void HandleResumeButtonClick()
    {
        Close();
        hub.Open(true);
    }

    private void HandleQuitButtonClick()
    {
        timeScaleManager?.Resume();
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

