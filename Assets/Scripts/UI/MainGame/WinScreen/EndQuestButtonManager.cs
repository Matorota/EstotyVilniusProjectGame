using UnityEngine;
using UnityEngine.UI;

public class EndQuestButtonManager : MonoBehaviour
{
    [SerializeField] private Button endQuestButton;
  //  [SerializeField] private GameObject winScreenRoot;
   // [SerializeField] private GameObject hudWindowRoot;
   // [SerializeField] private PauseMenu pauseMenu;

    private void Start()
    {
        if (endQuestButton == null)
        {
            return;
        }

        endQuestButton.gameObject.SetActive(false);
        
        endQuestButton.onClick.AddListener(OnEndQuestButtonClicked);
    }

    public void ShowEndQuestButton()
    {
        if (endQuestButton == null)
        {
            return;
        }

        
        endQuestButton.gameObject.SetActive(true);
        endQuestButton.interactable = true;
        
        CanvasGroup buttonCanvasGroup = endQuestButton.GetComponent<CanvasGroup>();
        if (buttonCanvasGroup != null)
        {
            buttonCanvasGroup.alpha = 1f;
            buttonCanvasGroup.interactable = true;
            buttonCanvasGroup.blocksRaycasts = true;
        }

        Transform current = endQuestButton.transform.parent;
        while (current != null)
        {
            CanvasGroup parentCanvasGroup = current.GetComponent<CanvasGroup>();
            if (parentCanvasGroup != null)
            {
                parentCanvasGroup.alpha = 1f;
                parentCanvasGroup.interactable = true;
                parentCanvasGroup.blocksRaycasts = true;
            }
            
            current.gameObject.SetActive(true);
            current = current.parent;
        }

    }

    public void HideEndQuestButton()
    {
        if (endQuestButton == null)
        {
            return;
        }

        endQuestButton.gameObject.SetActive(false);
    }

    public void OnEndQuestButtonClicked()
    {
        /*
        if (winScreenRoot != null)
        {
            winScreenRoot.SetActive(false);
        }
        
        if (hudWindowRoot != null)
        {
            hudWindowRoot.SetActive(false);
            Debug.Log("EndQuestButtonManager: HUD hidden");
        }*/
        
        HideEndQuestButton();
        /*
        if (pauseMenu != null)
        {
            pauseMenu.OpenAdditionalPanel();
            Debug.Log("EndQuestButtonManager: Guild panel opened!");
        }
        else
        {
            Debug.LogWarning("EndQuestButtonManager: PauseMenu not assigned!");
        }*/
    }
}
