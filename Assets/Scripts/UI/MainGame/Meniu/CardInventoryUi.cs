using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryUi : MonoBehaviour
{
    [SerializeField] private CardInventory inventory;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject[] cardPrefabs;
    private readonly Dictionary<string, Texture> textureByCardId = new Dictionary<string, Texture>();
    private readonly Dictionary<string, CardEntry> entriesByCardId = new Dictionary<string, CardEntry>();
    private readonly HashSet<string> missingTextureWarnings = new HashSet<string>();

    private sealed class CardEntry
    {
        public GameObject Root;
        public RawImage Image;
    }

    private void Awake()
    {
        inventory ??= FindObjectOfType<CardInventory>();
        cardsContainer ??= transform;
        CacheCardTextures();
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (inventory == null || cardsContainer == null) return;
        RefreshImageEntries();
    }

    private void CacheCardTextures()
    {
        textureByCardId.Clear();

        if (cardPrefabs == null)
        {
            return;
        }

        foreach (GameObject cardPrefab in cardPrefabs)
        {
            if (cardPrefab == null) continue;
            CardPickup pickup = cardPrefab.GetComponent<CardPickup>();
            RawImage rawImage = cardPrefab.GetComponentInChildren<RawImage>(true);
            if (pickup == null || string.IsNullOrWhiteSpace(pickup.CardId) || rawImage == null || rawImage.texture == null)
            {
                continue;
            }

            textureByCardId[pickup.CardId] = rawImage.texture;
        }
    }

    private void RefreshImageEntries()
    {
        int visibleIndex = 0;

        foreach (var pair in inventory.CardCountsById)
        {
            string cardId = pair.Key;
            int count = pair.Value;
            if (count <= 0) continue;

            if (!entriesByCardId.TryGetValue(cardId, out CardEntry entry))
            {
                entry = CreateEntry(cardId);
                entriesByCardId[cardId] = entry;
            }

            if (entry == null || entry.Root == null) continue;

            if (entry.Image != null && entry.Image.texture == null && textureByCardId.TryGetValue(cardId, out Texture texture))
            {
                entry.Image.texture = texture;
            }

            entry.Root.SetActive(true);
            SetEntryPosition(entry.Root.GetComponent<RectTransform>(), visibleIndex);
            visibleIndex++;
        }

        foreach (var pair in entriesByCardId)
        {
            string cardId = pair.Key;
            CardEntry entry = pair.Value;
            bool hasCard = inventory.CardCountsById.TryGetValue(cardId, out int count) && count > 0;
            if (entry != null && entry.Root != null) entry.Root.SetActive(hasCard);
        }
    }

    private CardEntry CreateEntry(string cardId)
    {
        GameObject root = new GameObject($"Card_{cardId}", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        root.transform.SetParent(cardsContainer, false);

        RawImage image = root.GetComponent<RawImage>();
        if (textureByCardId.TryGetValue(cardId, out Texture texture))
        {
            image.texture = texture;
        }
        else if (missingTextureWarnings.Add(cardId))
        {
            Debug.LogWarning($"{nameof(CardInventoryUi)}: No image texture found for cardId '{cardId}'. Add matching prefab to Card Prefabs.");
        }

        image.color = Color.white;

        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(96f, 96f);

        return new CardEntry
        {
            Root = root,
            Image = image
        };
    }

    private static void SetEntryPosition(RectTransform rect, int index)
    {
        if (rect == null) return;
        const float cell = 108f;
        int col = index % 6;
        int row = index / 6;
        rect.anchoredPosition = new Vector2(col * cell, -row * cell);
    }
}
