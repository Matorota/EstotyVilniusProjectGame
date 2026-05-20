﻿using System.Collections;
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
        [SerializeField] private CountEnemies countEnemies;
        [SerializeField] private float refreshInterval = 0.5f;

        private QuestConfig currentQuest;
        private Coroutine refreshCoroutine;

        public void Setup(QuestConfig quest)
        {
            currentQuest = quest;
            if (nameText != null)
                nameText.text = quest != null ? quest.Name : string.Empty;

            if (enemiesSlider != null)
            {
                enemiesSlider.maxValue = quest != null && quest.EnemiesAmount > 0 ? quest.EnemiesAmount : 1;
            }

            UpdateEnemiesText();
        }

        private void OnEnable()
        {
            if (countEnemies == null)
            {
                countEnemies = FindObjectOfType<CountEnemies>();
            }

            if (refreshCoroutine == null)
                refreshCoroutine = StartCoroutine(RefreshRoutine());
        }

        private void OnDisable()
        {
            if (refreshCoroutine != null)
            {
                StopCoroutine(refreshCoroutine);
                refreshCoroutine = null;
            }
        }

        private IEnumerator RefreshRoutine()
        {
            while (true)
            {
                UpdateEnemiesText();
                yield return new WaitForSeconds(refreshInterval);
            }
        }

        private void UpdateEnemiesText()
        {
            int alive = 0;
            if (countEnemies != null)
            {
                alive = countEnemies.CountAliveEnemies();
            }
            else
            {
                // fallback: scan Health components
                var all = FindObjectsOfType<Health>();
                for (int i = 0; i < all.Length; i++)
                {
                    if (all[i] != null && all[i].Team == Team.Enemy && all[i].CurrentHealth > 0f)
                        alive++;
                }
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
                enemiesSlider.value = alive;
            }
        }
    }
}