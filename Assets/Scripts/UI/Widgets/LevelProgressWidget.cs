using System.Collections.Generic;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Widgets
{
    public class LevelProgressWidget : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Slider enemiesSlider;
        [SerializeField] private GameObject background;

        private QuestConfig currentQuest;
        private  HashSet<Health> trackedEnemies = new HashSet<Health>();

        private void OnEnable()
        {
            EnemySpawner.OnEnemySpawned += OnEnemySpawned;

            RegisterExistingEnemies();

            UpdateEnemiesText();
        }

        private void OnDisable()
        {
            EnemySpawner.OnEnemySpawned -= OnEnemySpawned;
            UnregisterAllEnemies();
        }

        private void RegisterExistingEnemies()
        {
            var roots = gameObject.scene.GetRootGameObjects();  // cannot think of another way how to do it without findobjectbytype
            for (int r = 0; r < roots.Length; r++)
            {
                var found = roots[r].GetComponentsInChildren<Health>(true);
                for (int i = 0; i < found.Length; i++)
                {
                    var h = found[i];
                    if (h != null && h.Team == Team.Enemy)
                        RegisterEnemy(h);
                }
            }
        }

        private void RegisterEnemy(Health h)
        {
            if (h == null || trackedEnemies.Contains(h)) return;
            trackedEnemies.Add(h);
            h.OnDeath += OnTrackedEnemyDeath;
            UpdateEnemiesText();
        }

        private void UnregisterAllEnemies()
        {
            foreach (var h in trackedEnemies)
            {
                if (h != null) h.OnDeath -= OnTrackedEnemyDeath;
            }
            trackedEnemies.Clear();
        }

        private void OnEnemySpawned(Health h)
        {
            RegisterEnemy(h);
        }

        private void OnTrackedEnemyDeath()
        {
            UpdateEnemiesText();
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
        }

        private void UpdateEnemiesText()
        {
            int alive = 0;
            foreach (var h in trackedEnemies)
            {
                if (h != null && h.CurrentHealth > 0f) alive++;
            }

            if (descriptionText != null)
            {
                if (currentQuest != null)
                    descriptionText.text = $"Enemies: {alive}/{currentQuest.EnemiesAmount}";
                else
                    descriptionText.text = $"Enemies: {alive}";
            }

            if (enemiesSlider != null)
            {
                float val = Mathf.Clamp(alive, 0, (int)enemiesSlider.maxValue);
                enemiesSlider.value = val;
                GameObject sliderGO = enemiesSlider.gameObject;
                bool shouldShow = alive > 0;
                if (sliderGO != this.gameObject)
                {
                    sliderGO.SetActive(shouldShow);
                }
                else
                {
                    CanvasGroup component = sliderGO.GetComponent<CanvasGroup>();
                    if (component == null) component = sliderGO.AddComponent<CanvasGroup>();
                    component.alpha = shouldShow ? 1f : 0f;
                    component.interactable = shouldShow;
                    component.blocksRaycasts = shouldShow;
                }
            }

            if (background != null)
            {
                GameObject bgGO = background.gameObject;
                bool bgShow = alive > 0;
                if (bgGO != this.gameObject)
                {
                    bgGO.SetActive(bgShow);
                }
                else
                {
                    CanvasGroup bgCg = bgGO.GetComponent<CanvasGroup>();
                    if (bgCg == null) bgCg = bgGO.AddComponent<CanvasGroup>();
                    bgCg.alpha = bgShow ? 1f : 0f;
                    bgCg.interactable = bgShow;
                    bgCg.blocksRaycasts = bgShow;
                }
            }
        }
    }
}