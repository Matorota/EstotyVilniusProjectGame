using Characters.Player.Inventory;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardWidget : MonoBehaviour
{
    [SerializeField] private CardConfig config;
    private CardInventory inventory;
    [SerializeField] private CardType cardType;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;

    public CardConfig Config => config;
    public CardType CardType => cardType;

    private void Awake()
    {
        ResolveConfigFromInventory();
        Refresh();
    }

    public void SetConfig(CardConfig cardConfig)
    {
        config = cardConfig;
        Refresh();
    }

    public void UseSpeedUpConfig()
    {
        cardType = CardType.SpeedUp;
        config = null;
        Refresh();
    }

    public void UseDamageConfig()
    {
        cardType = CardType.Damage;
        config = null;
        Refresh();
    }

    public void UseHealthConfig()
    {
        cardType = CardType.Health;
        config = null;
        Refresh();
    }

    public void Refresh()
    {
        ResolveConfigFromInventory();

        if (config == null)
        {
            return;
        }

        if (nameText != null)
        {
            nameText.text = config.Name;
        }

        if (descriptionText != null)
        {
            descriptionText.text = config.Description;
        }

        if (iconImage != null)
        {
            iconImage.sprite = config.Image;
        }
    }

    private void ResolveConfigFromInventory()
    {
        if (config != null)
        {
            return;
        }

        inventory ??= GetComponentInParent<CardInventory>();
        if (inventory != null)   
        {
            config = inventory.GetCardConfig(cardType);
        }
    }
}
