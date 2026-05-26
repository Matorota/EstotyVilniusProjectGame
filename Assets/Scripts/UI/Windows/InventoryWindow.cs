using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private GameObject hudWindowGameObject;
    [SerializeField] private Button closeInventoryButton;

    private void OnEnable()
    {
        if (hudWindowGameObject != null)
            hudWindowGameObject.SetActive(false);

        if (closeInventoryButton != null)
            closeInventoryButton.onClick.AddListener(HandleCloseButtonClick);
    }

    private void OnDisable()
    {
        if (closeInventoryButton != null)
            closeInventoryButton.onClick.RemoveListener(HandleCloseButtonClick);
    }

    private void HandleCloseButtonClick()
    {
        if (hudWindowGameObject != null)
            hudWindowGameObject.SetActive(true);
        
        gameObject.SetActive(false);
    }
}
