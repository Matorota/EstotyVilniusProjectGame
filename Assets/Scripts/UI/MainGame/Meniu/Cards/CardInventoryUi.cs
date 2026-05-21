using System;
using Characters.Player.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryUi : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;
    [SerializeField] private SelectedCardsManager selectedCardsManager;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private CardWidget cardPrefab;
    [SerializeField] private Button openQuestButton;
    [SerializeField] private MenuController menuController;

    private void OnEnable()
    {
        openQuestButton.onClick.AddListener(HandleOpenQuestButtonClicked);
        inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        openQuestButton.onClick.RemoveListener(HandleOpenQuestButtonClicked);
        inventory.OnInventoryChanged -= Refresh;
    }

    private void Refresh()
    {
        for (int i = cardsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsContainer.GetChild(i).gameObject);
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
        selectedCardsManager.TryEquip(model);
    }
    

    private void HandleOpenQuestButtonClicked()
    {
        menuController.OpenAdditionalPanel();
        gameObject.SetActive(false);
    }
}

