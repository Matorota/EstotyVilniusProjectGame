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
    [SerializeField] private Button reOpenQuestButton;
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private GameObject hudWindowRoot;

    private void OnEnable()
    {
        if (reOpenQuestButton != null)
            reOpenQuestButton.onClick.AddListener(HandleReOpenQuestButtonClicked);
        inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (reOpenQuestButton != null)
            reOpenQuestButton.onClick.RemoveListener(HandleReOpenQuestButtonClicked);
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
        gameUIController.OpenAdditionalPanel();
        gameObject.SetActive(false);
    }
    
    private void HandleReOpenQuestButtonClicked()
    {
        gameUIController.CloseAdditionalPanel();
        gameUIController.Resume();
        gameObject.SetActive(false);
    }
}

