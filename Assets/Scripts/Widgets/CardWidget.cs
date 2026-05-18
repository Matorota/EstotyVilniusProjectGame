using Characters.Player.Inventory;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardWidget : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;

    private CardModel cardModel;
    private CardConfig displayConfig;
    private System.Action<CardModel> clickHandler;

    public CardModel Model => cardModel;
    public CardConfig Config => cardModel?.config ?? displayConfig;
    public CardType CardType => Config != null ? Config.Type : default;

    private void Awake()
    {
        EnsureBindings();
    }

    public void Setup(CardConfig config)
    {
        if (config == null)
        {
            return;
        }

        EnsureBindings();
        displayConfig = config;
        if (nameText != null) nameText.text = config.Name;
        if (descriptionText != null) descriptionText.text = config.Description;
        if (iconImage != null) iconImage.sprite = config.Image;
    }

    public void Bind(CardModel model, System.Action<CardModel> onClick)
    {
        cardModel = model;
        clickHandler = onClick;
        displayConfig = model?.config;
        Setup(model?.config);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardModel == null || clickHandler == null)
        {
            return;
        }

        clickHandler.Invoke(cardModel);
    }

    private void EnsureBindings()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        if (nameText == null)
        {
            nameText = FindTextByName(texts, "name", "title") ?? (texts.Length > 0 ? texts[0] : null);
        }

        if (descriptionText == null)
        {
            descriptionText = FindTextByName(texts, "description", "desc") ?? (texts.Length > 1 ? texts[1] : null);
        }

        if (iconImage == null)
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            iconImage = FindImageByName(images, "icon");
            if (iconImage == null)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    if (images[i] != null)
                    {
                        iconImage = images[i];
                        break;
                    }
                }
            }
        }
    }

    private TMP_Text FindTextByName(TMP_Text[] texts, params string[] tokens)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];
            if (text == null)
            {
                continue;
            }

            string lower = text.gameObject.name.ToLowerInvariant();
            for (int j = 0; j < tokens.Length; j++)
            {
                if (lower.Contains(tokens[j]))
                {
                    return text;
                }
            }
        }

        return null;
    }

    private Image FindImageByName(Image[] images, string token)
    {
        for (int i = 0; i < images.Length; i++)
        {
            Image image = images[i];
            if (image == null)
            {
                continue;
            }

            if (image.gameObject.name.ToLowerInvariant().Contains(token))
            {
                return image;
            }
        }

        return null;
    }
}
