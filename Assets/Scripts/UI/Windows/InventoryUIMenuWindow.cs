using UnityEngine;
using UnityEngine.UI;

public class InventoryUIMenuWindow : MonoBehaviour
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