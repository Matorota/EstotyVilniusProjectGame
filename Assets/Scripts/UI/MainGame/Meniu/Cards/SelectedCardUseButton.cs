using System;
using UnityEngine;
using UnityEngine.UI;


public class SelectedCardUseButton : MonoBehaviour
{
    [SerializeField] private Button button;
    private GameObject activeIndicator; 

    private SelectedCardsManager manager;
    private Characters.Player.Inventory.CardType type;

    public void Initialize(SelectedCardsManager manager, Characters.Player.Inventory.CardType type)
    {
        this.manager = manager;
        this.type = type;

        if (button == null) button = GetComponent<Button>() ?? GetComponentInChildren<Button>(true);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }

        Refresh();

        if (manager != null) manager.OnSelectedChanged += Refresh;
    }

    private void OnDestroy()
    {
        if (manager != null) manager.OnSelectedChanged -= Refresh;
        if (button != null) button.onClick.RemoveAllListeners();
    }

    private void OnButtonClicked()
    {
        if (manager == null) return;
        manager.TryUse(type);
    }

    private void Refresh()
    {
        if (manager == null)
        {
            gameObject.SetActive(false);
            return;
        }

        var selected = manager.GetSelectedCards();
        var entry = selected.Find(e => e.Type == type);
        bool isEquipped = entry.Config != null || entry.Type == type; 

        // show button only when the card is currently equipped
        gameObject.SetActive(isEquipped);

        bool isActive = entry.IsActive;
        if (button != null)
        {
            button.interactable = isEquipped && !isActive;
        }

        if (activeIndicator != null)
        {
            activeIndicator.SetActive(isActive);
        }
    }
}