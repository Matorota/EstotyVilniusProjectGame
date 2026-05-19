using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryWidget : UIWidgetBase
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Image artworkImage;

    private void Awake()
    {
        EnsureBindings();
    }

    public void Setup(StoryConfig story)
    {
        EnsureBindings();

        if (titleText != null) titleText.text = story?.Title ?? string.Empty;
        if (speakerText != null) speakerText.text = story?.Speaker ?? string.Empty;
        if (bodyText != null) bodyText.text = story?.Body ?? string.Empty;
        if (artworkImage != null) artworkImage.sprite = story?.Image;
    }

    private void EnsureBindings()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        if (titleText == null)
        {
            titleText = FindTextByName(texts, "title", "name") ?? (texts.Length > 0 ? texts[0] : null);
        }

        if (speakerText == null)
        {
            speakerText = FindTextByName(texts, "speaker", "who", "name");
        }

        if (bodyText == null)
        {
            bodyText = FindTextByName(texts, "body", "description", "desc") ?? (texts.Length > 1 ? texts[1] : null);
        }

        if (artworkImage == null)
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            artworkImage = (images.Length > 0) ? images[0] : null;
        }
    }
}