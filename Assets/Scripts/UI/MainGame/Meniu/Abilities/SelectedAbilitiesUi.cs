using UnityEngine;
using UnityEngine.UI;

public class SelectedAbilitiesUi : MonoBehaviour
{
    [SerializeField] private SelectedCardsManager manager;
    [SerializeField] private Button[] abilityButtons;

    private void Awake()
    {
        manager ??= FindObjectOfType<SelectedCardsManager>();
    }

    private void OnEnable()
    {
        if (manager != null)
        {
            manager.OnSelectedChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (manager != null)
        {
            manager.OnSelectedChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        if (manager == null || abilityButtons == null)
        {
            return;
        }

        var selected = manager.GetSelectedCards();

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            Button button = abilityButtons[i];
            if (button == null)
            {
                continue;
            }

            if (i >= selected.Count)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            var entry = selected[i];
            button.gameObject.SetActive(true);
            button.interactable = !entry.IsActive;

            int slotIndex = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => manager.TryUseByIndex(slotIndex));

            Text label = button.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.text = GetAbilityText(i, entry);
            }
        }
    }

    private string GetAbilityText(int slotIndex, SelectedCardsManager.SelectedCardInfo entry)
    {
        if (entry.Config == null)
        {
            return $"Ability {slotIndex + 1}";
        }

        return $"Ability {slotIndex + 1}: +{entry.Config.Value} {entry.Config.Type}";
    }
}
