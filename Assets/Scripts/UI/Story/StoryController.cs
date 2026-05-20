using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (storyWidget == null) return;
        if (storyPages.Count == 0)
        {
            storyWidget.Setup(null);
            return;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, storyPages.Count - 1);
        storyWidget.Setup(storyPages[currentIndex]);
        
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

    private void LoadGameplay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameplayScene");
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
            GetComponent<CanvasGroup>().alpha = 0f;
            gameObject.SetActive(false);
        }
    }

    public void Previous()
    {
        if (storyPages.Count == 0 || !HasPrevious()) return;
        currentIndex--;
        ShowCurrent();
    }

    public void GoToIndex(int index)
    {
        if (storyPages.Count == 0) return;
        currentIndex = Mathf.Clamp(index, 0, storyPages.Count - 1);
        ShowCurrent();
    }

    public bool HasNext() => currentIndex < storyPages.Count - 1;
    public bool HasPrevious() => currentIndex > 0;
}
