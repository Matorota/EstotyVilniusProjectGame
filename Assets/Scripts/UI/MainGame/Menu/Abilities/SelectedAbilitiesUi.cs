using Characters.Player.Inventory;
using Configs;
using UnityEngine;
using UnityEngine.UI;

public class SelectedAbilitiesUi : MonoBehaviour
{
    [SerializeField] private SelectedCardsManager manager;
    [SerializeField] private Abilities abilitiesConfig;
    [SerializeField] private Button[] abilityButtons;

    private void OnEnable()
    {
        manager.OnSelectedChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        manager.OnSelectedChanged -= Refresh;
    }

    public void Refresh()
    {
        var selected = manager.GetSelectedCards();

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            Button button = abilityButtons[i];
            
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

            if (abilitiesConfig != null)
            {
                button.image.sprite = abilitiesConfig.GetImage(entry.Type);
            }

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

        return $"Ability {slotIndex + 1}: +{entry.Config.Value:0.##} {entry.Config.Type}";
    }
}
