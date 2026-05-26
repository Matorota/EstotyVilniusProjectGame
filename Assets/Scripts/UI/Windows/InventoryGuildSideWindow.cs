using UnityEngine;
using UnityEngine.UI;

public class InventoryGuildSideWindow : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button openGuildButton;
    [SerializeField] private GameObject inventoryMenuWindowGameObject;
    [SerializeField] private GameObject guildWindowGameObject;


    private void OnEnable()
    {
            menuButton.onClick.AddListener(HandleMenuButtonClick);
            openGuildButton.onClick.AddListener(HandleOpenGuildButtonClicked);
    }

    private void OnDisable()
    {
        menuButton.onClick.RemoveListener(HandleMenuButtonClick);
        openGuildButton.onClick.RemoveListener(HandleOpenGuildButtonClicked);
    }

    private void HandleMenuButtonClick()
    {
        inventoryMenuWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void HandleOpenGuildButtonClicked()
    {
        guildWindowGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}