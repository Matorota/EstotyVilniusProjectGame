using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class EndQuestWidget : MonoBehaviour
    {
        [SerializeField] private QuestRunner questRunner;
        [SerializeField] private Button endGameButton;
        [SerializeField] private GuildWindow guildWindow;
        private CanvasGroup canvasGroup;


        private void OnEnable()
        {
            ResolveQuestRunner();
            ResolveGuildWindow();
            ResolveEndGameButton();
            if (endGameButton != null)
                endGameButton.onClick.AddListener(HandleEndGameButtonClick);
        }

        private void OnDisable()
        {
            if (endGameButton != null)
                endGameButton.onClick.RemoveListener(HandleEndGameButtonClick);
        }

        private void HandleEndGameButtonClick()
        {
            HideButton();

            if (questRunner != null)
            {
                questRunner.EndQuest();
            }

            // Open Guild Window after quest ends
            if (guildWindow != null)
            {
                guildWindow.gameObject.SetActive(true);
                Debug.Log("GuildWindow opened");
            }
        }

        public void ShowButton()
        {
            Debug.Log("EndQuestWidget.ShowButton() called");
            ResolveEndGameButton();
            
            if (endGameButton == null)
            {
                Debug.LogError("endGameButton is NULL in ShowButton()!");
                return;
            }
            
            // Unhide button
            endGameButton.gameObject.SetActive(true);
            Debug.Log("EndQuestButton SetActive(true)");
            
            // Ensure CanvasGroup is visible
            EnsureCanvasGroup();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            // Unhide all parents that have CanvasGroups
            Transform parent = endGameButton.transform.parent;
            while (parent != null)
            {
                CanvasGroup parentCG = parent.GetComponent<CanvasGroup>();
                if (parentCG != null)
                {
                    parentCG.alpha = 1f;
                    parentCG.interactable = true;
                    parentCG.blocksRaycasts = true;
                    Debug.Log($"Unhid parent CanvasGroup on {parent.name}");
                }
                
                parent = parent.parent;
            }
            
            Debug.Log($"EndQuestButton CanvasGroup: alpha={canvasGroup.alpha}, interactable={canvasGroup.interactable}");
        }

        public void HideButton()
        {
            Debug.Log("EndQuestWidget.HideButton() called");
            ResolveEndGameButton();
            if (endGameButton == null)
                return;
                
            EnsureCanvasGroup();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            endGameButton.gameObject.SetActive(false);
        }

        private void EnsureCanvasGroup()
        {
            if (canvasGroup == null && endGameButton != null)
            {
                canvasGroup = endGameButton.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = endGameButton.gameObject.AddComponent<CanvasGroup>();
                    Debug.Log("Created CanvasGroup on EndQuestButton");
                }
            }
        }

        private void ResolveEndGameButton()
        {
            if (endGameButton != null)
                return;

            endGameButton = GetComponentInChildren<Button>(true);
            if (endGameButton == null)
                Debug.LogError("Could not find Button in EndQuestWidget!");
        }

        private void ResolveGuildWindow()
        {
            if (guildWindow != null)
                return;

            GameObject[] roots = gameObject.scene.GetRootGameObjects();
            for (int r = 0; r < roots.Length; r++)
            {
                guildWindow = roots[r].GetComponentInChildren<GuildWindow>(true);
                if (guildWindow != null)
                {
                    return;
                }
            }
        }

        private void ResolveQuestRunner()
        {
            if (questRunner != null)
            {
                return;
            }

            GameObject[] roots = gameObject.scene.GetRootGameObjects();
            for (int r = 0; r < roots.Length; r++)
            {
                questRunner = roots[r].GetComponentInChildren<QuestRunner>(true);
                if (questRunner != null)
                {
                    return;
                }
            }
        }
    }
}