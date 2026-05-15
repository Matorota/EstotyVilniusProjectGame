using System;
using System.Collections.Generic;
using Characters.Player.Inventory;
using UnityEngine;

public class SelectedCardsUi : MonoBehaviour
{
    [SerializeField] private SelectedCardsManager manager;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject[] cardPrefabs;
    private Dictionary<CardType, GameObject> prefabByCardType = new Dictionary<CardType, GameObject>();

    private void Awake()
    {
        manager ??= FindObjectOfType<SelectedCardsManager>();
        cardsContainer ??= transform;
        CachePrefabs();
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

    private void CachePrefabs()
    {
        prefabByCardType.Clear();
        if (cardPrefabs == null) return;
        foreach (var p in cardPrefabs)
        {
            if (p == null) continue;
            CardWidget widget = p.GetComponent<CardWidget>() ?? p.GetComponentInChildren<CardWidget>(true);
            if (widget?.Config != null) prefabByCardType[widget.Config.Type] = p;
        }
    }

    private void Refresh()
    {
        // clear existing children
        if (cardsContainer == null) return;
        for (int i = cardsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsContainer.GetChild(i).gameObject);
        }

        if (manager == null) return;

        // Use detailed selected info so the displayed selected card matches the exact instance (config + texture)
        var selectedDetails = manager.GetSelectedCards();
        foreach (var entry in selectedDetails)
        {
            CardType type = entry.Type;
            if (prefabByCardType.TryGetValue(type, out var prefab))
            {
                GameObject root = Instantiate(prefab, cardsContainer);
                root.name = $"Selected_{type}_{Guid.NewGuid():N}";

                // Disable equipping on the displayed selected card and add unequip handler
                CardWidget w = root.GetComponent<CardWidget>() ?? root.GetComponentInChildren<CardWidget>(true);
                if (w != null)
                {
                    w.CanEquip = false;
                    if (entry.Config != null)
                    {
                        w.SetConfig(entry.Config);
                    }
                    if (entry.Texture != null)
                    {
                        w.SetTexture(entry.Texture);
                    }
                }

                SelectedCardWidget scw = root.GetComponent<SelectedCardWidget>() ?? root.GetComponentInChildren<SelectedCardWidget>(true);
                if (scw == null)
                {
                    root.AddComponent<SelectedCardWidget>();
                }
            }
        }
    }
}
