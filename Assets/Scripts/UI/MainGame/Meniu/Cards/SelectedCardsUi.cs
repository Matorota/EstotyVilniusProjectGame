using System;
using UnityEngine;

public class SelectedCardsUi : MonoBehaviour
{
    [SerializeField] private SelectedCardsManager manager;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private CardWidget cardPrefab;

    private void Awake()
    {
        manager ??= FindObjectOfType<SelectedCardsManager>();
        cardsContainer ??= transform;
    }

    private void OnEnable()
    {
        if (manager != null) manager.OnSelectedChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (manager != null) manager.OnSelectedChanged -= Refresh;
    }

    private void Refresh()
    {
        if (cardsContainer == null) return;
        for (int i = cardsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsContainer.GetChild(i).gameObject);
        }

        if (manager == null || cardPrefab == null) return;

        var selectedDetails = manager.GetSelectedCards();
        foreach (var entry in selectedDetails)
        {
            if (entry.Model == null || entry.Config == null)
            {
                continue;
            }

            CardWidget widget = Instantiate(cardPrefab, cardsContainer);
            widget.gameObject.name = $"Selected_{entry.Type}_{Guid.NewGuid():N}";
            widget.Bind(entry.Model, HandleSelectedCardClick);
        }
    }

    private void HandleSelectedCardClick(Characters.Player.Inventory.CardModel model)
    {
        if (manager == null || model == null)
        {
            return;
        }

        manager.TryUnequip(model);
    }
}