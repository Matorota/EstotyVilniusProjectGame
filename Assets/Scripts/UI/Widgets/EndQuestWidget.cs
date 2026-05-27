using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class EndQuestWidget : MonoBehaviour
    {
        [SerializeField] private QuestRunner questRunner;
        [SerializeField] private Button endGameButton;
        [SerializeField] private GuildWindow guildWindow;
        [SerializeField] private PlayerLifecycle playerLifecycle;
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
            DestroyAllCardPickups();
            DisablePlayerMovement();
            questRunner.EndQuest();
            guildWindow.gameObject.SetActive(true);
        }

        public void ShowButton()
        {
            ResolveEndGameButton();
            endGameButton.gameObject.SetActive(true);
            EnsureCanvasGroup();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            Transform parent = endGameButton.transform.parent;
            while (parent != null)
            {
                CanvasGroup parentCG = parent.GetComponent<CanvasGroup>();
                if (parentCG != null)
                {
                    parentCG.alpha = 1f;
                    parentCG.interactable = true;
                    parentCG.blocksRaycasts = true;
                }
                parent = parent.parent;
            }
        }

        public void HideButton()
        {
            ResolveEndGameButton();
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
                }
            }
        }

        private void ResolveEndGameButton()
        {
            if (endGameButton != null)
                return;
            endGameButton = GetComponentInChildren<Button>(true);
        }

        private void ResolveGuildWindow()
        {
            if (guildWindow != null)
                return;

            GameObject[] roots = gameObject.scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                guildWindow = root.GetComponentInChildren<GuildWindow>(true);
                if (guildWindow != null)
                    return;
            }
        }

        private void DestroyAllCardPickups()
        {
            CardPickup[] allPickups = FindObjectsByType<CardPickup>(FindObjectsSortMode.None);
            CharacterMovements player = FindObjectOfType<CharacterMovements>();
            
            foreach (CardPickup pickup in allPickups)
            {
                if (pickup != null && pickup.gameObject != player?.gameObject)
                {
                    Destroy(pickup.gameObject);
                }
            }
        }

        private void ResolveQuestRunner()
        {
            if (questRunner != null)
                return;

            GameObject[] roots = gameObject.scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                questRunner = root.GetComponentInChildren<QuestRunner>(true);
                if (questRunner != null)
                    return;
            }
        }

        private void DisablePlayerMovement()
        {
            ResolvePlayerLifecycle();
            playerLifecycle?.DisableMovement();
        }

        private void ResolvePlayerLifecycle()
        {
            if (playerLifecycle != null)
                return;

            CharacterMovements charMovements = FindObjectOfType<CharacterMovements>();
            playerLifecycle = charMovements?.GetComponent<PlayerLifecycle>();
        }
    }
}