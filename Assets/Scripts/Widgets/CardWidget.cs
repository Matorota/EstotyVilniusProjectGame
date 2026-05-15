using System;
using Characters.Player.Inventory;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardWidget : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardConfig config;
    [SerializeField] private CardType cardType;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;

    public CardConfig Config => config;
    public CardType CardType => cardType;
    public bool CanEquip { get; set; } = true;

    private int instanceIndex = -1;
    public int InstanceIndex => instanceIndex;

    private void Awake()
    {
        Refresh();
    }

    public void SetConfig(CardConfig cardConfig)
    {
        config = cardConfig;
        if (cardConfig != null)
        {
            cardType = cardConfig.Type;
        }
        Refresh();
    }

    public void Refresh()
    {
        config ??= GetComponentInParent<CardInventory>()?.GetCardConfig(cardType);
        if (config == null) return;

        if (nameText != null) nameText.text = config.Name;
        if (descriptionText != null) descriptionText.text = config.Description;
        if (iconImage != null) iconImage.sprite = config.Image;
    }

    public void SetTexture(Texture texture)
    {
        if (iconImage == null || texture is not Texture2D tex2D) return;

        try
        {
            iconImage.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        }
        catch (Exception)
        {
        }
    }

    public void SetInstanceIndex(int index)
    {
        instanceIndex = index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanEquip) return;

        SelectedCardsManager manager = FindObjectOfType<SelectedCardsManager>();
        if (manager == null) return;

        bool equipped = instanceIndex >= 0 ? manager.TryEquip(cardType, instanceIndex) : manager.TryEquip(cardType);
        if (equipped) gameObject.SetActive(false);
    }
}
