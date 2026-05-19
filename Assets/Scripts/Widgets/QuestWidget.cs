using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestWidget : UIWidgetBase, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;

    private QuestConfig questConfig;
    private System.Action<QuestConfig> clickHandler;

    public QuestConfig Config => questConfig;

    private void Awake()
    {
        EnsureBindings();
    }

    public void Setup(QuestConfig config)
    {
        if (config == null)
        {
            return;
        }

        EnsureBindings();
        questConfig = config;
        if (nameText != null) nameText.text = config.Name;
        if (descriptionText != null) descriptionText.text = config.Description;
        if (iconImage != null) iconImage.sprite = config.Image;
    }

    public void Bind(QuestConfig config, System.Action<QuestConfig> onClick)
    {
        questConfig = config;
        clickHandler = onClick;
        Setup(config);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (questConfig == null || clickHandler == null)
        {
            return;
        }

        clickHandler.Invoke(questConfig);
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

}
