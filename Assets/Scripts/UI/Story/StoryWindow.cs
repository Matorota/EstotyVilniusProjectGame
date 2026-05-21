using System.Collections.Generic;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryWindow : MonoBehaviour
{
    [SerializeField] private List<StoryPageConfig> storyPages = new();
    [SerializeField] private Button nextStoryButton;
    [SerializeField] private Button previousStoryButton;
    [SerializeField] private Button playStoryButton;
    [SerializeField] private GameUIController gameUIController;
    [SerializeField] private int startIndex;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Image artworkImage;
    private int currentIndex = -1;

    private void OnEnable()
    {
        AttachButtonHandlers();
        storyPages.RemoveAll(page => page == null);
        EnsureBindings();
        
        if (currentIndex == -1)
        {
            currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, storyPages.Count - 1));
        }
        else
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, storyPages.Count - 1));
        }
        
        ShowCurrentPage();
    }

    private void OnDisable()
    {
        DetachButtonHandlers();
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

    private TMP_Text FindTextByName(TMP_Text[] texts, params string[] names)
    {
        foreach (TMP_Text text in texts)
        {
            foreach (string name in names)
            {
                if (text.gameObject.name.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return text;
                }
            }
        }
        return null;
    }

    private void SetupPage(StoryPageConfig storyPage)
    {
        EnsureBindings();

        if (titleText != null) titleText.text = storyPage?.Title ?? string.Empty;
        if (speakerText != null) speakerText.text = storyPage?.Speaker ?? string.Empty;
        if (bodyText != null) bodyText.text = storyPage?.Body ?? string.Empty;
        if (artworkImage != null) artworkImage.sprite = storyPage?.Image;
    }

    private void ShowCurrentPage()
    {
        currentIndex = Mathf.Clamp(currentIndex, 0, storyPages.Count - 1);
        
        if (storyPages.Count > 0)
        {
            SetupPage(storyPages[currentIndex]);
        }
        
        UpdateButtonStates();
    }

    public void Next()
    {
        if (!HasNext()) 
            return;
        
        currentIndex++;
        ShowCurrentPage();
    }

    public void Previous()
    {
        if (storyPages.Count == 0)
        {
            UpdateButtonStates();
            return;
        }

        currentIndex = Mathf.Max(0, currentIndex - 1);
        ShowCurrentPage();
    }

    public void PlayStory()
    {
        gameUIController.OpenAdditionalPanel();
        gameObject.SetActive(false);
    }

    public void UpdateButtonStates()
    {
        bool hasNext = HasNext();

        nextStoryButton.gameObject.SetActive(true);
        nextStoryButton.interactable = hasNext;
        
        previousStoryButton.gameObject.SetActive(true);
        previousStoryButton.interactable = HasPrevious();
        
        playStoryButton.gameObject.SetActive(true);
        bool playInteractable = !hasNext;
        playStoryButton.interactable = playInteractable;
    }

    public void OnNextButtonClicked()
    {
        Next();
        UpdateButtonStates();
    }

    public void OnPreviousButtonClicked()
    {
        Previous();
        UpdateButtonStates();
    }

    public void OnPlayButtonClicked()
    {
        PlayStory();
    }

    public bool HasNext() => currentIndex < storyPages.Count - 1;
    public bool HasPrevious() => currentIndex > 0;

    private void AttachButtonHandlers()
    {
        nextStoryButton.onClick.RemoveListener(OnNextButtonClicked);
        nextStoryButton.onClick.AddListener(OnNextButtonClicked);
        
        previousStoryButton.onClick.RemoveListener(OnPreviousButtonClicked);
        previousStoryButton.onClick.AddListener(OnPreviousButtonClicked);
        
        playStoryButton.onClick.RemoveListener(OnPlayButtonClicked);
        playStoryButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void DetachButtonHandlers()
    {
        nextStoryButton.onClick.RemoveListener(OnNextButtonClicked); 
        previousStoryButton.onClick.RemoveListener(OnPreviousButtonClicked); 
        playStoryButton.onClick.RemoveListener(OnPlayButtonClicked);
    }
}

