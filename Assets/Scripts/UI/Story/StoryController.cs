using System.Collections.Generic;
using Configs;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    [SerializeField] private List<StoryConfig> storyPages = new List<StoryConfig>();
    [SerializeField] private StoryWidget storyWidget;
    [SerializeField] private int startIndex = 0;

    private int currentIndex = 0;

    private void Awake()
    {
        if (storyWidget == null)
        {
            storyWidget = GetComponentInChildren<StoryWidget>(true);
        }

        currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, storyPages.Count - 1));
    }

    private void OnEnable()
    {
        ShowCurrent();
    }

    public void ShowCurrent()
    {
        if (storyWidget == null) return;
        if (storyPages == null || storyPages.Count == 0)
        {
            storyWidget.Setup(null);
            return;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, storyPages.Count - 1);
        storyWidget.Setup(storyPages[currentIndex]);
    }

    public void Next()
    {
        if (storyPages == null || storyPages.Count == 0) return;
        if (currentIndex >= storyPages.Count - 1) return;
        currentIndex++;
        ShowCurrent();
    }

    public void Previous()
    {
        if (storyPages == null || storyPages.Count == 0) return;
        if (currentIndex <= 0) return;
        currentIndex--;
        ShowCurrent();
    }

    public void GoToIndex(int index)
    {
        if (storyPages == null || storyPages.Count == 0) return;
        currentIndex = Mathf.Clamp(index, 0, storyPages.Count - 1);
        ShowCurrent();
    }

    public bool HasNext() => storyPages != null && currentIndex < storyPages.Count - 1;
    public bool HasPrevious() => storyPages != null && currentIndex > 0;
}
