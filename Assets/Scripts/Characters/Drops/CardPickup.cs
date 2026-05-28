using System.Collections.Generic;
using Characters.Player.Inventory;
using Configs;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class CardPickup : MonoBehaviour
{
    private static  HashSet<CardPickup> ActivePickups = new(); // I need to do static without it i need to use FindObjectsOfType so one or the other i do not know witch is the better option

     private CardConfig cardConfig;
     private CardWidget cardWidget;
     private RawImage worldIconRawImage;

    private CardWidget[] widgets;
    private bool canBePickedUp;

    public static List<CardPickup> GetActivePickups()
    {
        return new List<CardPickup>(ActivePickups);
    }

    private void Reset()
    {
        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    private void Awake()
    {
        cardWidget ??= GetComponent<CardWidget>() ?? GetComponentInChildren<CardWidget>(true);
        worldIconRawImage ??= GetComponentInChildren<RawImage>(true);
        widgets = GetComponentsInChildren<CardWidget>(true);
        ApplyConfigToVisuals();
    }

    private void OnEnable()
    {
        ActivePickups.Add(this);
        canBePickedUp = false;
        Invoke(nameof(EnablePickup), 0.3f);
    }

    private void EnablePickup()
    {
        canBePickedUp = true;
    }

    private void OnDisable()
    {
        ActivePickups.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBePickedUp)
            return;

        CardInventory inventory =
            other.GetComponent<CardInventory>() ??
            other.GetComponentInParent<CardInventory>();

        if (inventory == null)
        {
            return;
        }

        CardConfig configToCollect = cardConfig ?? cardWidget?.Config;
        if (configToCollect == null)
        {
            return;
        }

        if (inventory.Collect(new CardModel { config = configToCollect }))
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(CardConfig config)
    {
        Debug.Log($"[CardPickup] Initialize called with config: {(config != null ? config.Name : "NULL")}", this);
        cardConfig = config;
        cardWidget ??= GetComponent<CardWidget>() ?? GetComponentInChildren<CardWidget>(true);
        worldIconRawImage ??= GetComponentInChildren<RawImage>(true);
        widgets = GetComponentsInChildren<CardWidget>(true);
        ApplyConfigToVisuals();
    }

    private void ApplyConfigToVisuals()
    {
        if (cardConfig == null)
        {
            Debug.LogWarning("[CardPickup] cardConfig is null. Cannot apply visuals.", this);
            return;
        }

        Debug.Log($"[CardPickup] Applying visuals for {cardConfig.Name}. cardWidget={(cardWidget != null)} worldIcon={(worldIconRawImage != null)} widgets={(widgets != null ? widgets.Length : 0)}", this);

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"[CardPickup] Canvas renderMode={canvas.renderMode} scale={canvas.transform.localScale}", this);
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning("[CardPickup] Canvas is NOT in WorldSpace mode! Card won't be visible in world.", this);
            }
        }
        else
        {
            Debug.LogWarning("[CardPickup] No Canvas found on prefab! Add a Canvas in WorldSpace mode.", this);
        }

        if (cardWidget != null)
        {
            cardWidget.Setup(cardConfig);
        }

        if (widgets != null)
        {
            for (int i = 0; i < widgets.Length; i++)
            {
                CardWidget widget = widgets[i];
                if (widget != null)
                {
                    widget.Setup(cardConfig);
                }
            }
        }

        if (cardConfig.Image != null)
        {
            if (worldIconRawImage != null)
            {
                worldIconRawImage.texture = cardConfig.Image.texture;
                Debug.Log($"[CardPickup] Set RawImage texture to {cardConfig.Image.name}", this);
            }
            else
            {
                Debug.LogWarning("[CardPickup] worldIconRawImage is null. Card image won't show.", this);
            }

            if (worldIconRawImage == null || canvas == null || canvas.renderMode != RenderMode.WorldSpace)
            {
                EnsureSpriteRendererFallback(cardConfig.Image);
            }
        }
        else
        {
            Debug.LogWarning("[CardPickup] cardConfig.Image is null. No visual to display.", this);
        }
    }

    private void EnsureSpriteRendererFallback(Sprite sprite)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
            Debug.Log("[CardPickup] Added SpriteRenderer as fallback visual.", this);
        }
        sr.sprite = sprite;
        sr.sortingOrder = 100;
        Debug.Log($"[CardPickup] SpriteRenderer fallback set to {sprite.name}", this);
    }
}
