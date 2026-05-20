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
        if (nextButton == null || playButton == null) return;

        if (storyController == null)
        {
            storyController = GetComponentInParent<StoryController>();
        }

        bool hasNext = storyController != null && storyController.HasNext();

        nextButton.gameObject.SetActive(true);
        nextButton.interactable = true;

        playButton.gameObject.SetActive(true);
        bool playInteractable = !hasNext;
        playButton.interactable = playInteractable;

        Image playImage = playButton.GetComponent<Image>();
        if (playImage != null)
        {
            Color c = playImage.color;
            c.a = playInteractable ? 1f : 0.5f;
            playImage.color = c;
        }
        else
        {
            CanvasGroup cg = playButton.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = playInteractable ? 1f : 0.5f;
        }
    }

    public void OnNextButtonClicked()
    {
        if (storyController != null)
        {
            storyController.Next();
            UpdateButtonStates();
        }
    }

    public void OnPreviousButtonClicked()
    {
        if (storyController != null)
        {
            storyController.Previous();
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
