using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private Button menuButton;

    private void OnEnable()
    {
        if (menuButton != null)
            menuButton.onClick.AddListener(HandleInventoryButtonClick);
    }

    private void OnDisable()
    {
        if (menuButton != null)
            menuButton.onClick.RemoveListener(HandleInventoryButtonClick);
    }

    private void HandleInventoryButtonClick()
    {
        gameUIController.OpenOtherPanel();
    }
}