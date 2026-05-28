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

    public void Setup(QuestConfig config)
    {
        if (config == null)
            return;

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
            return;

        clickHandler.Invoke(questConfig);
    }
}
