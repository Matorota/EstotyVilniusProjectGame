using System;
using Characters.Player.Inventory;
using UnityEngine;

public class SelectedCardsUi : MonoBehaviour
{
    [SerializeField] private SelectedCardsManager manager;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private CardWidget cardPrefab;

    private void OnEnable()
    {
        manager.OnSelectedChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        manager.OnSelectedChanged -= Refresh;
    }

    private void Refresh()
    {
        for (int i = cardsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsContainer.GetChild(i).gameObject);
        }

        var selectedDetails = manager.GetSelectedCards();
        foreach (var entry in selectedDetails)
        {
            CardWidget widget = Instantiate(cardPrefab, cardsContainer);
            widget.gameObject.name = $"Selected_{entry.Type}_{Guid.NewGuid():N}";
            widget.Bind(entry.Model, HandleSelectedCardClick);
        }
    }

    private void HandleSelectedCardClick(Characters.Player.Inventory.CardModel model)
    {
        manager.TryUnequip(model);
    }
}