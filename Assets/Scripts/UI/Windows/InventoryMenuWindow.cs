using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuWindow : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject inventoryWindowGameObject;

    private void OnEnable()
    {
        if (backButton != null)
            backButton.onClick.AddListener(HandleBackButtonClick);
    }

    private void OnDisable()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(HandleBackButtonClick);
    }

    private void HandleBackButtonClick()
    {
        if (inventoryWindowGameObject != null)
            inventoryWindowGameObject.SetActive(true);
        
        gameObject.SetActive(false);
    }
}
