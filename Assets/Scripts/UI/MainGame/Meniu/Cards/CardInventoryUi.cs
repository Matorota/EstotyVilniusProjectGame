using System;
using Characters.Player.Inventory;
using UnityEngine;

public class CardInventoryUi : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;
    [SerializeField] private SelectedCardsManager selectedCardsManager;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private CardWidget cardPrefab;

    private void Awake()
    {
        inventory ??= FindObjectOfType<CardInventory>();
        selectedCardsManager ??= FindObjectOfType<SelectedCardsManager>();
        cardsContainer ??= transform;
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (cardsContainer == null)
        {
            return;
        }

        for (int i = cardsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsContainer.GetChild(i).gameObject);
        }

        if (inventory == null || cardPrefab == null)
        {
            return;
        }

        foreach (CardModel model in inventory.GetUnequippedCards())
        {
            CardWidget widget = Instantiate(cardPrefab, cardsContainer);
            widget.gameObject.name = $"Card_{model.config.Type}_{Guid.NewGuid():N}";
            widget.Bind(model, HandleCardClick);
        }
    }

    private void HandleCardClick(CardModel model)
    {
        if (selectedCardsManager == null || model == null)
        {
            return;
        }

        selectedCardsManager.TryEquip(model);
    }
}
