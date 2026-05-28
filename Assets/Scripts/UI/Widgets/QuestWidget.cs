using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestWidget : MonoBehaviour, IPointerClickHandler
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
    {            if (nameText == null || descriptionText == null || iconImage == null)
        {
            Debug.LogError($"{nameof(QuestWidget)} on '{gameObject.name}' requires nameText, descriptionText and iconImage assigned in the Inspector.");
        }
    }

    private TMP_Text FindTextByName(TMP_Text[] texts, params string[] names)
    {
        if (texts == null) return null;
        foreach (TMP_Text text in texts)
        {
            if (text == null) continue;
            foreach (string name in names)
            {
                if (!string.IsNullOrEmpty(name) && text.gameObject.name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return text;
                }
            }
        }
        return null;
    }

    private Image FindImageByName(Image[] images, string name)
    {
        if (images == null) return null;
        foreach (Image img in images)
        {
            if (img == null) continue;
            if (!string.IsNullOrEmpty(name) && img.gameObject.name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return img;
            }
        }
        return null;
    }

}
