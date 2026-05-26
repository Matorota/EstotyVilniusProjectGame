using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuWindow : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject inventoryWindowGameObject;

    private void OnEnable()
    {
        backButton.onClick.AddListener(HandleBackButtonClick);
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(HandleBackButtonClick);
    }

    private void HandleBackButtonClick()
    {
        inventoryWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
