using UnityEngine;
using UnityEngine.UI;

public class EndQuestButtonManager : MonoBehaviour
{
   [SerializeField] private Button endQuestButton;
   [SerializeField] private GameObject winScreenRoot;
   [SerializeField] private GameObject hudWindowRoot;
   [SerializeField] private PauseMenu pauseMenu;

   private void Start()
   {
       if (endQuestButton == null) return;

       endQuestButton.gameObject.SetActive(false);
       endQuestButton.onClick.AddListener(OnEndQuestButtonClicked);
   }

   public void ShowEndQuestButton()
   {
       if (endQuestButton == null) return;

       endQuestButton.gameObject.SetActive(true);
       endQuestButton.interactable = true;
        
       SetCanvasGroupVisible(endQuestButton.GetComponent<CanvasGroup>(), true);
       SetParentCanvasGroupsVisible(endQuestButton.transform, true);
   }

   public void HideEndQuestButton()
   {
       if (endQuestButton != null)
       {
           endQuestButton.gameObject.SetActive(false);
       }
   }

   private void OnEndQuestButtonClicked()
   {
       SetActiveIfAssigned(winScreenRoot, false);
       SetActiveIfAssigned(hudWindowRoot, false);
       HideEndQuestButton();
        
       if (pauseMenu != null)
       {
           pauseMenu.OpenAdditionalPanel();
       }
   }

   private void SetCanvasGroupVisible(CanvasGroup canvasGroup, bool visible)
   {
       if (canvasGroup == null) return;
       canvasGroup.alpha = visible ? 1f : 0f;
       canvasGroup.interactable = visible;
       canvasGroup.blocksRaycasts = visible;
   }

   private void SetParentCanvasGroupsVisible(Transform target, bool visible)
   {
       Transform current = target.parent;
       while (current != null)
       {
           CanvasGroup canvasGroup = current.GetComponent<CanvasGroup>();
           if (canvasGroup != null)
           {
               SetCanvasGroupVisible(canvasGroup, visible);
           }
           current.gameObject.SetActive(visible || current.gameObject.activeSelf);
           current = current.parent;
       }
   }

   private void SetActiveIfAssigned(GameObject target, bool isActive)
   {
       if (target != null) target.SetActive(isActive);
   }
}
