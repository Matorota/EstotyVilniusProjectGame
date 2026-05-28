using Configs;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class EndQuestWidget : MonoBehaviour
    {
        public static EndQuestWidget Instance { get; private set; }
        [SerializeField] private QuestRunner questRunner;
        [SerializeField] private Button endGameButton;
        [SerializeField] private GuildWindow guildWindow;
        [SerializeField] private PlayerLifecycle playerLifecycle;
        private CanvasGroup canvasGroup;


        private void OnEnable()
        {
            ResolveEndGameButton();
            if (endGameButton != null)
                endGameButton.onClick.AddListener(HandleEndGameButtonClick);

            if (questRunner == null)
                questRunner = QuestRunner.Instance;
            
            
            if (questRunner != null)
            {
                questRunner.OnQuestStarted += HandleQuestStarted;
                questRunner.OnQuestEnded += HandleQuestEnded;
            }
        }

        private void OnDisable()
        {
            if (endGameButton != null)
                endGameButton.onClick.RemoveListener(HandleEndGameButtonClick);

            if (questRunner != null)
            {
                questRunner.OnQuestStarted -= HandleQuestStarted;
                questRunner.OnQuestEnded -= HandleQuestEnded;
            }
        }
        private void Awake()
        {
            Instance = this;
            questRunner ??= QuestRunner.Instance; 
        }
        private void HandleEndGameButtonClick()
        {
            if (questRunner != null)
                questRunner.EndQuest();

            if (guildWindow != null)
                guildWindow.gameObject.SetActive(true);
        }

        private void HandleQuestStarted(QuestConfig config)
        {
            HideButton();
        }

        private void HandleQuestEnded()
        {
            HideButton();
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
                if (!parent.gameObject.activeSelf)
                    parent.gameObject.SetActive(true);

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
    }
}