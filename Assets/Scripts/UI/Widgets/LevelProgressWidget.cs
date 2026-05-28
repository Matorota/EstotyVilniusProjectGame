using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Widgets
{
    public class LevelProgressWidget : MonoBehaviour
    {
        [SerializeField] private QuestRunner questRunner;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Slider enemiesSlider;
        [SerializeField] private GameObject background;

        private QuestConfig currentQuest;

        private void OnEnable()
        {
            if (questRunner == null)
            {
                Debug.LogError("LevelProgressWidget: assign QuestRunner in Inspector.");
                Setup(null);
                return;
            }

            questRunner.OnQuestStarted += HandleQuestStarted;
            questRunner.OnAliveCountChanged += HandleAliveCountChanged;
            questRunner.OnQuestEnded += HandleQuestEnded;
            SyncToQuestRunner();
        }

        private void OnDisable()
        {
            if (questRunner != null)
            {
                questRunner.OnQuestStarted -= HandleQuestStarted;
                questRunner.OnAliveCountChanged -= HandleAliveCountChanged;
                questRunner.OnQuestEnded -= HandleQuestEnded;
            }
        }

        public void Setup(QuestConfig quest)
        {
            currentQuest = quest;
            if (nameText != null)
                nameText.text = quest != null ? quest.Name : string.Empty;

            if (enemiesSlider != null)
            {
                enemiesSlider.maxValue = quest != null && quest.EnemiesAmount > 0 ? quest.EnemiesAmount : 1;
                enemiesSlider.value = 0f;
            }

            UpdateEnemiesText();
            SetProgressVisible(quest != null);
        }

        private void UpdateEnemiesText()
        {
            int alive = questRunner != null ? questRunner.AliveEnemies : 0;
            int total = questRunner != null ? questRunner.TotalEnemies : (currentQuest != null ? currentQuest.EnemiesAmount : 0);

            if (descriptionText != null)
            {
                if (currentQuest != null)
                    descriptionText.text = $"Enemies: {alive}/{total}";
                else
                    descriptionText.text = string.Empty;
            }

            if (enemiesSlider != null)
            {
                enemiesSlider.maxValue = Mathf.Max(1, total);
                enemiesSlider.value = Mathf.Clamp(alive, 0, (int)enemiesSlider.maxValue);
            }

            if (background != null)
            {
                SetTargetVisible(background, currentQuest != null);
            }
        }

        private void HandleQuestStarted(QuestConfig quest)
        {
            Setup(quest);
        }

        private void HandleAliveCountChanged()
        {
            UpdateEnemiesText();
        }

        private void HandleQuestEnded()
        {
            Setup(null);
        }

        private void SyncToQuestRunner()
        {
            if (questRunner == null || questRunner.CurrentQuest == null)
            {
                Setup(null);
                return;
            }

            Setup(questRunner.CurrentQuest);
        }

        private void SetProgressVisible(bool visible)
        {
            if (enemiesSlider != null)
            {
                SetTargetVisible(enemiesSlider.gameObject, visible);
            }

            if (background != null)
            {
                SetTargetVisible(background, visible);
            }
        }

        private void SetTargetVisible(GameObject target, bool visible)
        {
            if (target == null)
            {
                return;
            }

            if (target != gameObject)
            {
                target.SetActive(visible);
                return;
            }

            CanvasGroup component = target.GetComponent<CanvasGroup>();
            if (component == null)
            {
                component = target.AddComponent<CanvasGroup>();
            }

            component.alpha = visible ? 1f : 0f;
            component.interactable = visible;
            component.blocksRaycasts = visible;
        }

    }
}