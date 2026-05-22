using System.Collections.Generic;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryWindow : MonoBehaviour
{
    [SerializeField] private List<StoryPageConfig> storyPages = new();
    
    [SerializeField] private GameUIController gameUIController;
    
    [SerializeField] private Button nextStoryButton;
    [SerializeField] private Button previousStoryButton;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text bodyText;

    [SerializeField] private TMP_Text nextButtonText;
    
    [SerializeField] private Image storyImage;
    
    private int currentIndex;

    private void OnEnable()
    {
        nextStoryButton.onClick.AddListener(HandleNextButtonClicked);
        previousStoryButton.onClick.AddListener(HandlePreviousButtonClicked);

        ShowCurrentPage();
    }

    private void OnDisable()
    {
        nextStoryButton.onClick.RemoveListener(HandleNextButtonClicked); 
        previousStoryButton.onClick.RemoveListener(HandlePreviousButtonClicked); 
    }
    
    private void ShowCurrentPage()
    {
        currentIndex = Mathf.Clamp(currentIndex, 0, storyPages.Count - 1);
        StoryPageConfig storyPage = storyPages[currentIndex];
        
        titleText.text = storyPage.Title;
        speakerText.text = storyPage.Speaker;
        bodyText.text = storyPage.Body;
        storyImage.sprite = storyPage.Sprite;
        
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        previousStoryButton.gameObject.SetActive(IsFirstPage() == false);
        nextStoryButton.gameObject.SetActive(HasNextPage() || IsLastPage());
        
        nextButtonText.text = IsLastPage() ? "Play" : "Next";
    }

    private void GoToNextPage()
    {
        if (!HasNextPage()) 
            return;
        
        currentIndex++;
        ShowCurrentPage();
    }

    private void GoToPreviousPage()
    {
        if (IsFirstPage())
            return;

        currentIndex--;
        ShowCurrentPage();
    }

    private void HandleNextButtonClicked()
    {
        if (IsLastPage())
        {
            gameUIController.OpenAdditionalPanel();
            gameObject.SetActive(false);
        }
        else
        {
            GoToNextPage();
        }
    }

    private void HandlePreviousButtonClicked()
    {
        GoToPreviousPage();
    }

    private bool HasNextPage()
    {
        return currentIndex < storyPages.Count - 1;
    }
    
    private bool IsFirstPage()
    {
        return currentIndex == 0;
    }

    private bool IsLastPage()
    {
        return currentIndex == storyPages.Count - 1;
    }
}

