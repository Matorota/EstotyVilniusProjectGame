using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectedAbilitiesUi : MonoBehaviour
{
    [SerializeField] private SelectedCardsManager manager;
    [SerializeField] private Button[] abilityButtons; 
    private Text[] abilityLabels; 

    private void Awake()
    {
        manager ??= FindObjectOfType<SelectedCardsManager>();
    }

    private void OnEnable()
    {
        if (manager != null) manager.OnSelectedChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (manager != null) manager.OnSelectedChanged -= Refresh;
    }

    public void Refresh()
    {
        if (manager == null) return;
        var selected = manager.GetSelectedCards();

        int maxButtons = abilityButtons != null ? abilityButtons.Length : 0;
        for (int i = 0; i < maxButtons; i++)
        {
            Button btn = abilityButtons[i];
            Text lbl = (abilityLabels != null && i < abilityLabels.Length) ? abilityLabels[i] : null;

            if (i < selected.Count)
            {
                var entry = selected[i];
                if (btn != null)
                {
                    btn.gameObject.SetActive(true);
                    btn.onClick.RemoveAllListeners();
                    int idx = i; 
                    btn.onClick.AddListener(() => manager.TryUseByIndex(idx));
                    btn.interactable = !entry.IsActive;
                }

                if (lbl != null)
                {
                    string stat = entry.Config != null ? entry.Config.Type.ToString() : "";
                    int value = entry.Config != null ? entry.Config.Value : 0;
                    float dur = entry.Config != null ? entry.Config.Duration : 0f;
                    lbl.text = $"Ability {i+1}: +{value} {stat} ({dur:F0}s)";
                }
            }
            else
            {
                if (btn != null) btn.gameObject.SetActive(false);
                if (lbl != null) lbl.text = string.Empty;
            }
        }
    }
}