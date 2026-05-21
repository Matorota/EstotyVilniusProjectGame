using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.UI;

public class StoryWindow : MonoBehaviour
{
    [SerializeField] private List<StoryPageConfig> storyPages = new();
    [SerializeField] private StoryWidget storyWidget;
    [SerializeField] private Button nextStoryButton;
    [SerializeField] private Button previousStoryButton;
    [SerializeField] private Button playStoryButton;
    [SerializeField] private MenuController menuController;
    [SerializeField] private int startIndex;

    private int currentIndex = -1;

    private void OnEnable()
    {
        AttachButtonHandlers();
        storyPages.RemoveAll(page => page == null);
        
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

    private void ShowCurrentPage()
    {

        currentIndex = Mathf.Clamp(currentIndex, 0, storyPages.Count - 1);
        storyWidget.Setup(storyPages[currentIndex]);
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

        menuController.OpenAdditionalPanel();
        
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
