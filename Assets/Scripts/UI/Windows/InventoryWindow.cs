using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private GameObject hudWindowGameObject;
    [SerializeField] private Button closeInventoryButton;
    [SerializeField] private UI.Windows.GameHubWindow hub;

    private void OnEnable()
    {
        hub.Close();
        closeInventoryButton.onClick.AddListener(HandleCloseButtonClick);
    }

    private void OnDisable()
    {
        closeInventoryButton.onClick.RemoveListener(HandleCloseButtonClick);
    }

    private void HandleCloseButtonClick()
    {
        hub.Open();
        gameObject.SetActive(false);
    }
}
