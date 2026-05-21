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

    public void Setup(StoryPageConfig storyPage)
    {
        EnsureBindings();

        if (titleText != null) titleText.text = storyPage?.Title ?? string.Empty;
        if (speakerText != null) speakerText.text = storyPage?.Speaker ?? string.Empty;
        if (bodyText != null) bodyText.text = storyPage?.Body ?? string.Empty;
        if (artworkImage != null) artworkImage.sprite = storyPage?.Image;
    }

    private void EnsureBindings()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        
        titleText = FindTextByName(texts, "title", "name") ?? (texts.Length > 0 ? texts[0] : null);
        
        
        speakerText = FindTextByName(texts, "speaker", "who", "name");

        bodyText = FindTextByName(texts, "body", "description", "desc") ?? (texts.Length > 1 ? texts[1] : null);
        

        Image[] images = GetComponentsInChildren<Image>(true);
        artworkImage = (images.Length > 0) ? images[0] : null;
        
    }
}