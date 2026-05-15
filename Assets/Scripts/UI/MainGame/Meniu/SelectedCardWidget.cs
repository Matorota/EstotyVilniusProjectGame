using UnityEngine;
using UnityEngine.EventSystems;
using Characters.Player.Inventory;

// Click handler for cards shown in the Selected (equipped) area.
// Clicking will ask SelectedCardsManager to unequip the card (return to inventory).
public class SelectedCardWidget : MonoBehaviour, IPointerClickHandler
{
    private CardWidget cardWidget;
    private SelectedCardsManager manager;

    private void Awake()
    {
        cardWidget = GetComponent<CardWidget>() ?? GetComponentInChildren<CardWidget>(true);
        manager = FindObjectOfType<SelectedCardsManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardWidget == null)
        {
            Debug.LogWarning("SelectedCardWidget: no CardWidget found on selected card instance.");
            return;
        }
        if (manager == null)
        {
            Debug.LogWarning("SelectedCardWidget: no SelectedCardsManager found in scene.");
            return;
        }

        CardType type = cardWidget.CardType;
        bool ok = manager.TryUnequip(type);
        Debug.Log(ok ? $"Unequipped {type}" : $"Failed to unequip {type}");
    }
}
