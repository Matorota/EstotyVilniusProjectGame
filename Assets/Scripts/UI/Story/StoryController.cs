using System.Collections.Generic;
using Configs;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    [SerializeField] private List<StoryConfig> storyPages = new List<StoryConfig>();
    [SerializeField] private StoryWidget storyWidget;
    [SerializeField] private int startIndex = 0;

    private int currentIndex = 0;
    private StoryButtonManager buttonManager;

    private void Awake()
    {
        if (storyWidget == null)
        {
            storyWidget = GetComponentInChildren<StoryWidget>(true);
        }

        buttonManager = GetComponentInChildren<StoryButtonManager>(true);
        currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, storyPages.Count - 1));
    }

    private void OnEnable()
    {
        ShowCurrent();
    }

    public void ShowCurrent()
    {
        // Always ensure button states are refreshed even if storyWidget is not assigned.
        if (storyPages.Count == 0)
        {
            if (storyWidget != null) storyWidget.Setup(null);
            if (buttonManager != null) buttonManager.UpdateButtonStates();
            return;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, storyPages.Count - 1);
        if (storyWidget != null)
        {
            storyWidget.Setup(storyPages[currentIndex]);
        }

        // Ensure we have a reference to the button manager (prefab bindings may call controller methods directly).
        if (buttonManager == null)
        {
            buttonManager = GetComponentInChildren<StoryButtonManager>(true);
        }

        if (buttonManager != null)
        {
            buttonManager.UpdateButtonStates();
        }
    }

    public void Next()
    {
        if (storyPages.Count == 0 || !HasNext()) return;
        currentIndex++;
        ShowCurrent();
    }

    public void Previous()
    {
        if (storyPages.Count == 0 || !HasPrevious()) return;
        currentIndex--;
        ShowCurrent();
    }

    public void PlayStory()
    {
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu != null)
        {
            pauseMenu.CloseStoryWindow();
            pauseMenu.OpenAdditionalPanel();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public bool HasNext() => currentIndex < storyPages.Count - 1;
    public bool HasPrevious() => currentIndex > 0;
}
