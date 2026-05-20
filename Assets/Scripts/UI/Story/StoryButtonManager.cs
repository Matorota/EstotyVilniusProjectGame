using UnityEngine;
using UnityEngine.UI;

public class StoryButtonManager : MonoBehaviour
{
    [SerializeField] private StoryController storyController;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button playButton;

    private void Awake()
    {
        if (storyController == null)
        {
            storyController = GetComponentInParent<StoryController>();
        }
    }

    private void OnEnable()
    {
        UpdateButtonStates();
    }

    public void UpdateButtonStates()
    {
        // Debug:
        Debug.Log($"StoryButtonManager - storyController: {(storyController != null ? "✓" : "✗")}, nextButton: {(nextButton != null ? "✓" : "✗")}, playButton: {(playButton != null ? "✓" : "✗")}");

        if (storyController == null)
        {
            Debug.LogWarning("StoryButtonManager: storyController is not assigned!");
            return;
        }
        
        if (nextButton == null)
        {
            Debug.LogWarning("StoryButtonManager: nextButton is not assigned!");
            return;
        }
        
        if (playButton == null)
        {
            Debug.LogWarning("StoryButtonManager: playButton is not assigned!");
            return;
        }

        bool hasNext = storyController.HasNext();
        Debug.Log($"StoryButtonManager - Current index: {storyController}, HasNext: {hasNext}");

        nextButton.gameObject.SetActive(hasNext);
        playButton.gameObject.SetActive(!hasNext);
        
        Debug.Log($"Next button active: {hasNext}, Play button active: {!hasNext}");
    }

    public void OnNextButtonClicked()
    {
        if (storyController != null)
        {
            storyController.Next();
            UpdateButtonStates();
        }
    }

    public void OnPlayButtonClicked()
    {
        if (storyController != null)
        {
            storyController.PlayStory(); 
        }
    }
}
