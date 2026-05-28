using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private GameObject hudWindowGameObject;
    [SerializeField] private Button closeInventoryButton;
    [SerializeField] private UI.Windows.GameHubWindow hub;
    [SerializeField] private TimeScale timeScaleManager;

    private void OnEnable()
    {
        timeScaleManager?.Pause();
        hub.Close();
        if (hudWindowGameObject != null)
            hudWindowGameObject.SetActive(false);
        closeInventoryButton.onClick.AddListener(HandleCloseButtonClick);
    }

    private void OnDisable()
    {
        timeScaleManager?.Resume();
        closeInventoryButton.onClick.RemoveListener(HandleCloseButtonClick);
    }

    private void HandleCloseButtonClick()
    {
        if (hudWindowGameObject != null)
            hudWindowGameObject.SetActive(true);
        hub.Open();
        gameObject.SetActive(false);
    }
}
